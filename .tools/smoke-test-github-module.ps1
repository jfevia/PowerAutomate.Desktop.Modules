param(
    [string]
    $TestRepoOwner = 'jfevia',

    [string]
    $TestRepoName = 'PowerAutomate.Desktop.Modules'
)

$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
$modulePath = Join-Path $root 'modules\Modules.GitHub.Actions\bin\Debug\PowerAutomate.Desktop.Modules.GitHub.Actions.dll'
if (-not (Test-Path $modulePath)) {
    throw "Module DLL not found at $modulePath. Run dotnet build first."
}

# Load assembly + dependencies into the current PowerShell session.
Add-Type -Path (Join-Path $root 'modules\Modules.GitHub.Actions\bin\Debug\Newtonsoft.Json.dll')
Add-Type -Path (Join-Path $root 'modules\Modules.GitHub.Actions\bin\Debug\Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.dll')
Add-Type -Path $modulePath

$ns = 'PowerAutomate.Desktop.Modules.GitHub.Actions'

function Get-ActionType([string] $TypeName) {
    $t = [Type]::GetType("$ns.$TypeName, PowerAutomate.Desktop.Modules.GitHub.Actions")
    if ($null -eq $t) { throw "Type $TypeName not found" }
    return $t
}

function Invoke-Action {
    param(
        [string] $TypeName,
        [hashtable] $Inputs = @{}
    )
    $type = Get-ActionType $TypeName
    $instance = [System.Activator]::CreateInstance($type)
    foreach ($k in $Inputs.Keys) {
        $prop = $type.GetProperty($k)
        if ($null -eq $prop) { throw "Property $k not found on $TypeName" }
        $prop.SetValue($instance, $Inputs[$k])
    }
    $executeMethod = $type.GetMethod('Execute')
    $sw = [System.Diagnostics.Stopwatch]::StartNew()
    $result = [pscustomobject]@{
        Action  = $TypeName
        Status  = 'OK'
        Elapsed = $null
        Error   = $null
        Output  = $null
    }
    try {
        $executeMethod.Invoke($instance, @($null)) | Out-Null
        $resultProp = $type.GetProperty('Result')
        if ($null -ne $resultProp) {
            $result.Output = $resultProp.GetValue($instance)
        }
    }
    catch {
        $result.Status = 'FAIL'
        $inner = $_.Exception
        while ($null -ne $inner.InnerException) { $inner = $inner.InnerException }
        $result.Error = $inner.Message
    }
    $sw.Stop()
    $result.Elapsed = $sw.ElapsedMilliseconds
    return $result
}

# --- Step 1: interactive login via the LoginAction (reads token from local gh CLI) ---
Write-Host '=== Signing in via the local gh CLI ===' -ForegroundColor Cyan
$loginType = Get-ActionType 'LoginAction'
$loginMode = [Enum]::Parse((Get-ActionType 'LoginMode'), 'GitHubCli')
$login = [System.Activator]::CreateInstance($loginType)
$loginType.GetProperty('Mode').SetValue($login, $loginMode)
$loginType.GetMethod('Execute').Invoke($login, @($null))
$authContext = $loginType.GetProperty('Authentication').GetValue($login)
$authLogin = $authContext.GetType().GetProperty('Login').GetValue($authContext)
$authBaseUrl = $authContext.GetType().GetProperty('BaseUrl').GetValue($authContext)
Write-Host "Authenticated as $authLogin against $authBaseUrl" -ForegroundColor Green

# --- Step 2: define the smoke-test matrix ---
$tests = @(
    # --- Pattern: GET with no path params ---
    @{ Name = 'UsersGetAuthenticatedAction';                  Inputs = @{ Authentication = $authContext } },
    @{ Name = 'RateLimitGetAction';                           Inputs = @{ Authentication = $authContext } },
    @{ Name = 'MetaGetAction';                                Inputs = @{ Authentication = $authContext } },
    @{ Name = 'MetaGetAllVersionsAction';                     Inputs = @{ Authentication = $authContext } },
    @{ Name = 'GitignoreGetAllTemplatesAction';               Inputs = @{ Authentication = $authContext } },
    @{ Name = 'LicensesGetAllCommonlyUsedAction';             Inputs = @{ Authentication = $authContext } },
    @{ Name = 'EmojisGetAction';                              Inputs = @{ Authentication = $authContext } },
    @{ Name = 'CodesOfConductGetAllCodesOfConductAction';     Inputs = @{ Authentication = $authContext } },
    @{ Name = 'ReposListForAuthenticatedUserAction';          Inputs = @{ Authentication = $authContext } },
    @{ Name = 'OrgsListForAuthenticatedUserAction';           Inputs = @{ Authentication = $authContext } },
    @{ Name = 'GistsListAction';                              Inputs = @{ Authentication = $authContext } },
    @{ Name = 'IssuesListAction';                             Inputs = @{ Authentication = $authContext } },
    @{ Name = 'ActivityListReposStarredByAuthenticatedUserAction'; Inputs = @{ Authentication = $authContext } },
    @{ Name = 'ActivityListWatchedReposForAuthenticatedUserAction'; Inputs = @{ Authentication = $authContext } },
    @{ Name = 'AppsGetAuthenticatedAction';                   Inputs = @{ Authentication = $authContext }; ExpectFail = $true; Reason = 'requires JWT, not user token' },

    # --- Pattern: GET with simple path param ---
    @{ Name = 'UsersGetByUsernameAction';                     Inputs = @{ Authentication = $authContext; Username = $TestRepoOwner } },
    @{ Name = 'UsersListFollowersForUserAction';              Inputs = @{ Authentication = $authContext; Username = $TestRepoOwner } },
    @{ Name = 'UsersListFollowingForUserAction';              Inputs = @{ Authentication = $authContext; Username = $TestRepoOwner } },
    @{ Name = 'ReposListForUserAction';                       Inputs = @{ Authentication = $authContext; Username = $TestRepoOwner } },
    @{ Name = 'GistsListForUserAction';                       Inputs = @{ Authentication = $authContext; Username = $TestRepoOwner } },
    @{ Name = 'LicensesGetAction';                            Inputs = @{ Authentication = $authContext; License = 'mit' } },
    @{ Name = 'GitignoreGetTemplateAction';                   Inputs = @{ Authentication = $authContext; Name = 'VisualStudio' } },
    @{ Name = 'ActivityListEventsForAuthenticatedUserAction'; Inputs = @{ Authentication = $authContext; Username = $TestRepoOwner } },
    @{ Name = 'ActivityListReceivedEventsForUserAction';      Inputs = @{ Authentication = $authContext; Username = $TestRepoOwner } },

    # --- Pattern: GET with multiple path params ---
    @{ Name = 'ReposGetAction';                               Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } },
    @{ Name = 'ReposListBranchesAction';                      Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } },
    @{ Name = 'ReposListCommitsAction';                       Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } },
    @{ Name = 'ReposListContributorsAction';                  Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } },
    @{ Name = 'ReposListTagsAction';                          Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } },
    @{ Name = 'ReposListLanguagesAction';                     Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } },
    @{ Name = 'ReposListWebhooksAction';                      Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } },
    @{ Name = 'ReposGetReadmeAction';                         Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } },
    @{ Name = 'IssuesListForRepoAction';                      Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } },
    @{ Name = 'IssuesListLabelsForRepoAction';                Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } },
    @{ Name = 'IssuesListMilestonesAction';                   Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } },
    @{ Name = 'PullsListAction';                              Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } },
    @{ Name = 'PullsGetAction';                               Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName; PullNumber = 28 } },
    @{ Name = 'PullsListFilesAction';                         Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName; PullNumber = 28 } },
    @{ Name = 'PullsListCommitsAction';                       Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName; PullNumber = 28 } },
    @{ Name = 'IssuesListCommentsForRepoAction';              Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } },
    @{ Name = 'ActionsListRepoWorkflowsAction';               Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } },
    @{ Name = 'ActionsListWorkflowRunsForRepoAction';         Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } },
    @{ Name = 'ActivityListStargazersForRepoAction';          Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } },
    @{ Name = 'ActivityListWatchersForRepoAction';            Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } },

    # --- Pattern: GET with enum query parameter (the schemas that broke NSwag oneOf) ---
    @{ Name = 'SecurityAdvisoriesListGlobalAdvisoriesAction'; Inputs = @{ Authentication = $authContext } },

    # --- Pattern: GET endpoints that return oneOf-laden payloads (Anonymous14 / Rules / Conditions2 territory) ---
    @{ Name = 'ReposGetRepoRulesetsAction';                   Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } },
    @{ Name = 'ReposGetBranchRulesAction';                    Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName; Branch = 'main' } },

    # --- Pattern: search endpoints (returns wrapped collection) ---
    @{ Name = 'SearchReposAction';                            Inputs = @{ Authentication = $authContext; Q = 'org:' + $TestRepoOwner } },
    @{ Name = 'SearchUsersAction';                            Inputs = @{ Authentication = $authContext; Q = $TestRepoOwner + ' in:login' } },
    @{ Name = 'SearchIssuesAndPullRequestsAction';            Inputs = @{ Authentication = $authContext; Q = 'repo:' + $TestRepoOwner + '/' + $TestRepoName + ' is:pr' } },
    @{ Name = 'SearchCommitsAction';                          Inputs = @{ Authentication = $authContext; Q = 'Module GitHub repo:' + $TestRepoOwner + '/' + $TestRepoName } },
    @{ Name = 'SearchLabelsAction';                           Inputs = @{ Authentication = $authContext; Q = 'bug'; RepositoryId = 0 }; ExpectFail = $true; Reason = 'needs real repo id; testing param wiring' },

    # --- Pattern: special accept-header endpoints / non-JSON responses ---
    @{ Name = 'ReposGetContentAction';                        Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName; Path = 'README.md' } },

    # --- Pattern: PUT with no body (idempotent mutation) ---
    @{ Name = 'ActivityStarRepoForAuthenticatedUserAction';   Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } },

    # --- Pattern: GET that confirms PUT side effect (204 / 404 boolean) ---
    @{ Name = 'ActivityCheckRepoIsStarredByAuthenticatedUserAction'; Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } },

    # --- Pattern: DELETE no body (idempotent mutation) ---
    @{ Name = 'ActivityUnstarRepoForAuthenticatedUserAction'; Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } }
)

# --- Step 3: run them ---
Write-Host ''
Write-Host '=== Running smoke tests ===' -ForegroundColor Cyan
$results = foreach ($t in $tests) {
    Write-Host "  -> $($t.Name) ..." -NoNewline
    $r = Invoke-Action -TypeName $t.Name -Inputs $t.Inputs
    $expectedStatus = if ($t.ContainsKey('ExpectFail')) { 'FAIL' } else { 'OK' }
    $r | Add-Member -NotePropertyName Expected -NotePropertyValue $expectedStatus
    $r | Add-Member -NotePropertyName Reason -NotePropertyValue ($t.Reason)
    if ($r.Status -eq $r.Expected) {
        if ($r.Expected -eq 'FAIL') {
            Write-Host " EXPECTED FAIL ($($r.Elapsed) ms)" -ForegroundColor DarkGray
        } else {
            Write-Host " OK ($($r.Elapsed) ms)" -ForegroundColor Green
        }
    } else {
        Write-Host " $($r.Status) ($($r.Elapsed) ms): $($r.Error)" -ForegroundColor Red
    }
    $r
}

Write-Host ''
Write-Host '=== Summary ===' -ForegroundColor Cyan
$expected = ($results | Where-Object { $_.Status -eq $_.Expected }).Count
$unexpected = ($results | Where-Object { $_.Status -ne $_.Expected }).Count
$color = if ($unexpected -eq 0) { 'Green' } else { 'Yellow' }
Write-Host "Behaved as expected: $expected / $($results.Count)" -ForegroundColor $color
Write-Host "Unexpected results: $unexpected"
if ($unexpected -gt 0) {
    Write-Host ''
    Write-Host 'Unexpected:' -ForegroundColor Red
    $results | Where-Object { $_.Status -ne $_.Expected } | ForEach-Object {
        Write-Host ("  {0,-60} expected={1} actual={2} :: {3}" -f $_.Action, $_.Expected, $_.Status, $_.Error) -ForegroundColor Red
    }
}

# --- Step 4: end-to-end body-mutation chain (POST -> GET -> PATCH -> DELETE -> verify gone) ---
Write-Host ''
Write-Host '=== Gist create/get/update/delete chain ===' -ForegroundColor Cyan

$createBody = [Newtonsoft.Json.Linq.JObject]::Parse('{"description":"smoke test","public":false,"files":{"smoke-test.txt":{"content":"hello from PowerAutomate.Desktop.Modules smoke test"}}}')
$create = Invoke-Action -TypeName 'GistsCreateAction' -Inputs @{ Authentication = $authContext; Body = $createBody }
Write-Host ("  GistsCreate                                                  -> {0}  ({1} ms){2}" -f $create.Status, $create.Elapsed, $(if ($create.Error) { ": $($create.Error)" } else { '' })) -ForegroundColor $(if ($create.Status -eq 'OK') { 'Green' } else { 'Red' })
$chainResults = @($create)

if ($create.Status -eq 'OK') {
        $gistId = $create.Output.Id

        $get = Invoke-Action -TypeName 'GistsGetAction' -Inputs @{ Authentication = $authContext; GistId = $gistId }
        Write-Host ("  GistsGet ({0})                                       -> {1}  ({2} ms){3}" -f $gistId, $get.Status, $get.Elapsed, $(if ($get.Error) { ": $($get.Error)" } else { '' })) -ForegroundColor $(if ($get.Status -eq 'OK') { 'Green' } else { 'Red' })
        $chainResults += $get

        $updateBody = [Newtonsoft.Json.Linq.JObject]::Parse('{"description":"smoke test (updated)"}')
        $update = Invoke-Action -TypeName 'GistsUpdateAction' -Inputs @{ Authentication = $authContext; GistId = $gistId; Body = $updateBody }
        Write-Host ("  GistsUpdate                                                  -> {0}  ({1} ms){2}" -f $update.Status, $update.Elapsed, $(if ($update.Error) { ": $($update.Error)" } else { '' })) -ForegroundColor $(if ($update.Status -eq 'OK') { 'Green' } else { 'Red' })
        $chainResults += $update

        $delete = Invoke-Action -TypeName 'GistsDeleteAction' -Inputs @{ Authentication = $authContext; GistId = $gistId }
        Write-Host ("  GistsDelete                                                  -> {0}  ({1} ms){2}" -f $delete.Status, $delete.Elapsed, $(if ($delete.Error) { ": $($delete.Error)" } else { '' })) -ForegroundColor $(if ($delete.Status -eq 'OK') { 'Green' } else { 'Red' })
        $chainResults += $delete

        $getAfterDelete = Invoke-Action -TypeName 'GistsGetAction' -Inputs @{ Authentication = $authContext; GistId = $gistId }
        $expectedAfterDelete = 'FAIL'
        Write-Host ("  GistsGet (after delete; expect FAIL)                         -> {0}  ({1} ms){2}" -f $getAfterDelete.Status, $getAfterDelete.Elapsed, $(if ($getAfterDelete.Status -ne $expectedAfterDelete) { " (UNEXPECTED, expected $expectedAfterDelete)" } else { '' })) -ForegroundColor $(if ($getAfterDelete.Status -eq $expectedAfterDelete) { 'DarkGray' } else { 'Red' })
        $chainResults += $getAfterDelete
}

$global:GitHubSmokeResults = $results
$global:GitHubSmokeAuth = $authContext
$global:GitHubSmokeChainResults = $chainResults
