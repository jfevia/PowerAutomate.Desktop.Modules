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
    @{ Name = 'ActivityUnstarRepoForAuthenticatedUserAction'; Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } },

    # --- Pattern: array query parameters (comma-separated -> repeated in URL) ---
    @{ Name = 'IssuesListForRepoAction';                      Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName; Labels = 'bug,enhancement' } },
    @{ Name = 'IssuesListForRepoAction';                      Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName; State = 'closed'; Sort = 'updated'; Direction = 'desc'; PerPage = [int]2; Page = [int]1 } },

    # --- Pattern: DateTime query parameter ---
    @{ Name = 'IssuesListForRepoAction';                      Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName; Since = [System.DateTime]::Parse('2025-01-01T00:00:00Z') } },

    # --- Pattern: explicit pagination (per_page + page) ---
    @{ Name = 'ReposListForAuthenticatedUserAction';          Inputs = @{ Authentication = $authContext; PerPage = [int]2; Page = [int]1 } },

    # --- Pattern: 404 error from real-but-missing resource ---
    @{ Name = 'ReposGetAction';                               Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = 'this-repo-does-not-exist-' + ([Guid]::NewGuid().ToString('N').Substring(0,8)) }; ExpectFail = $true; Reason = '404 on missing repo - exercises error propagation' },
    @{ Name = 'UsersGetByUsernameAction';                     Inputs = @{ Authentication = $authContext; Username = 'no-such-user-' + ([Guid]::NewGuid().ToString('N').Substring(0,8)) }; ExpectFail = $true; Reason = '404 on missing user' },

    # --- Pattern: MS-connector parity (operations enumerated on the connector docs page) ---
    @{ Name = 'IssuesGetAction';                              Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName; IssueNumber = [int]28 } },          # GetIssueNum
    @{ Name = 'IssuesListLabelsOnIssueAction';                Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName; IssueNumber = [int]28 } },          # GetIssueLabels
    @{ Name = 'IssuesListAssigneesAction';                    Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } },                                  # GetAssignees
    @{ Name = 'ReposListCollaboratorsAction';                 Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName } },                                  # ListCollaborators
    @{ Name = 'GitGetRefAction';                              Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName; Ref = 'heads/main' } },              # GetReference
    @{ Name = 'ReposCompareCommitsAction';                    Inputs = @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName; Basehead = 'main...feature/github-module' } } # CompareRepositoryCommits
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

# --- Step 4: output-assertion checks (verify roundtripped data, not just status) ---
Write-Host ''
Write-Host '=== Output assertions ===' -ForegroundColor Cyan

function Assert-Output {
        param([string] $Label, [bool] $Condition, [string] $Detail = '')
        if ($Condition) {
            Write-Host ("  {0,-70} PASS {1}" -f $Label, $(if ($Detail) { "($Detail)" } else { '' })) -ForegroundColor Green
        } else {
            Write-Host ("  {0,-70} FAIL {1}" -f $Label, $(if ($Detail) { "($Detail)" } else { '' })) -ForegroundColor Red
        }
        return $Condition
}

$assertionResults = @()

$me = Invoke-Action -TypeName 'UsersGetAuthenticatedAction' -Inputs @{ Authentication = $authContext }
$meLogin = if ($me.Output) { [string]$me.Output['login'] } else { '' }
$meType = if ($me.Output) { [string]$me.Output['type'] } else { '' }
$assertionResults += Assert-Output 'UsersGetAuthenticated.login matches expected user' ($meLogin -eq $TestRepoOwner) "got=$meLogin"
$assertionResults += Assert-Output 'UsersGetAuthenticated.type is User' ($meType -eq 'User') "got=$meType"

$repo = Invoke-Action -TypeName 'ReposGetAction' -Inputs @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName }
$assertionResults += Assert-Output 'ReposGet.Name matches' ($repo.Output.Name -eq $TestRepoName) "got=$($repo.Output.Name)"
$assertionResults += Assert-Output 'ReposGet.FullName matches' ($repo.Output.FullName -eq "$TestRepoOwner/$TestRepoName") "got=$($repo.Output.FullName)"

$branches = Invoke-Action -TypeName 'ReposListBranchesAction' -Inputs @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName }
$assertionResults += Assert-Output 'ReposListBranches returned at least 1 branch' ($branches.Output.Count -ge 1) "count=$($branches.Output.Count)"
$assertionResults += Assert-Output 'ReposListBranches includes main' (($branches.Output | Where-Object Name -EQ 'main').Count -eq 1)

$pr = Invoke-Action -TypeName 'PullsGetAction' -Inputs @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName; PullNumber = 28 }
$assertionResults += Assert-Output 'PullsGet.Number = 28' ($pr.Output.Number -eq 28) "got=$($pr.Output.Number)"
$assertionResults += Assert-Output 'PullsGet.State present' (-not [string]::IsNullOrEmpty($pr.Output.State)) "got=$($pr.Output.State)"

$ratelimit = Invoke-Action -TypeName 'RateLimitGetAction' -Inputs @{ Authentication = $authContext }
$assertionResults += Assert-Output 'RateLimit.Rate is non-null and Limit > 0' ($null -ne $ratelimit.Output.Rate -and $ratelimit.Output.Rate.Limit -gt 0) "limit=$($ratelimit.Output.Rate.Limit)"

# Paginated request returns exactly perPage items
$page1 = Invoke-Action -TypeName 'ReposListForAuthenticatedUserAction' -Inputs @{ Authentication = $authContext; PerPage = [int]2; Page = [int]1 }
$assertionResults += Assert-Output 'ReposListForAuthenticatedUser respects PerPage=2' ($page1.Output.Count -eq 2) "got=$($page1.Output.Count)"

$assertPass = ($assertionResults | Where-Object { $_ -eq $true }).Count
$assertTotal = $assertionResults.Count
Write-Host ("  -> {0} / {1} assertions passed" -f $assertPass, $assertTotal) -ForegroundColor $(if ($assertPass -eq $assertTotal) { 'Green' } else { 'Red' })

# --- Step 5: full issue lifecycle (POST -> GET -> POST comment -> POST labels -> PATCH -> verify state) ---
Write-Host ''
Write-Host '=== Issue lifecycle ===' -ForegroundColor Cyan

$marker = '[smoke-test-' + ([Guid]::NewGuid().ToString('N').Substring(0,8)) + ']'
$createIssueBody = [Newtonsoft.Json.Linq.JObject]::Parse((ConvertTo-Json @{
        title = "$marker auto-generated smoke-test issue"
        body  = "Auto-generated by .tools/smoke-test-github-module.ps1 to validate the issue lifecycle. Will be closed automatically."
} -Compress))
$createIssue = Invoke-Action -TypeName 'IssuesCreateAction' -Inputs @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName; Body = $createIssueBody }
Write-Host ("  IssuesCreate                                          -> {0} ({1} ms){2}" -f $createIssue.Status, $createIssue.Elapsed, $(if ($createIssue.Error) { ': ' + $createIssue.Error } else { '' })) -ForegroundColor $(if ($createIssue.Status -eq 'OK') { 'Green' } else { 'Red' })

if ($createIssue.Status -eq 'OK') {
        $issueNumber = [int]$createIssue.Output.Number
        Write-Host ("    -> created issue #$issueNumber, title='$($createIssue.Output.Title)'") -ForegroundColor DarkGray

        $getIssue = Invoke-Action -TypeName 'IssuesGetAction' -Inputs @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName; IssueNumber = $issueNumber }
        Write-Host ("  IssuesGet                                             -> {0} ({1} ms)" -f $getIssue.Status, $getIssue.Elapsed) -ForegroundColor $(if ($getIssue.Status -eq 'OK') { 'Green' } else { 'Red' })
        Write-Host ("    -> roundtrip Title equals created Title: {0}" -f ($getIssue.Output.Title -eq $createIssue.Output.Title)) -ForegroundColor DarkGray

        $commentBody = [Newtonsoft.Json.Linq.JObject]::Parse('{"body":"Auto-generated comment from smoke test."}')
        $createComment = Invoke-Action -TypeName 'IssuesCreateCommentAction' -Inputs @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName; IssueNumber = $issueNumber; Body = $commentBody }
        Write-Host ("  IssuesCreateComment                                   -> {0} ({1} ms){2}" -f $createComment.Status, $createComment.Elapsed, $(if ($createComment.Error) { ': ' + $createComment.Error } else { '' })) -ForegroundColor $(if ($createComment.Status -eq 'OK') { 'Green' } else { 'Red' })

        $listComments = Invoke-Action -TypeName 'IssuesListCommentsAction' -Inputs @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName; IssueNumber = $issueNumber }
        Write-Host ("  IssuesListComments                                    -> {0} ({1} ms), count={2}" -f $listComments.Status, $listComments.Elapsed, $listComments.Output.Count) -ForegroundColor $(if ($listComments.Status -eq 'OK' -and $listComments.Output.Count -ge 1) { 'Green' } else { 'Red' })

        $updateBody = [Newtonsoft.Json.Linq.JObject]::Parse((ConvertTo-Json @{
            title = "$marker auto-generated smoke-test issue (updated)"
            state = 'closed'
        } -Compress))
        $update = Invoke-Action -TypeName 'IssuesUpdateAction' -Inputs @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName; IssueNumber = $issueNumber; Body = $updateBody }
        Write-Host ("  IssuesUpdate (title + close)                          -> {0} ({1} ms)" -f $update.Status, $update.Elapsed) -ForegroundColor $(if ($update.Status -eq 'OK') { 'Green' } else { 'Red' })
        Write-Host ("    -> new state='$($update.Output.State)' new title='$($update.Output.Title)'") -ForegroundColor DarkGray

        $reopenBody = [Newtonsoft.Json.Linq.JObject]::Parse('{"state":"open"}')
        $reopen = Invoke-Action -TypeName 'IssuesUpdateAction' -Inputs @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName; IssueNumber = $issueNumber; Body = $reopenBody }
        Write-Host ("  IssuesUpdate (reopen)                                 -> {0} ({1} ms), state='{2}'" -f $reopen.Status, $reopen.Elapsed, $reopen.Output.State) -ForegroundColor $(if ($reopen.Status -eq 'OK' -and $reopen.Output.State -eq 'open') { 'Green' } else { 'Red' })

        $closeAgainBody = [Newtonsoft.Json.Linq.JObject]::Parse('{"state":"closed"}')
        $closeAgain = Invoke-Action -TypeName 'IssuesUpdateAction' -Inputs @{ Authentication = $authContext; Owner = $TestRepoOwner; Repo = $TestRepoName; IssueNumber = $issueNumber; Body = $closeAgainBody }
        Write-Host ("  IssuesUpdate (close again)                            -> {0} ({1} ms), state='{2}'" -f $closeAgain.Status, $closeAgain.Elapsed, $closeAgain.Output.State) -ForegroundColor $(if ($closeAgain.Status -eq 'OK' -and $closeAgain.Output.State -eq 'closed') { 'Green' } else { 'Red' })
}

# --- Step 6: end-to-end body-mutation chain (POST -> GET -> PATCH -> DELETE -> verify gone) ---
Write-Host ''
Write-Host '=== Gist create/get/update/delete chain ===' -ForegroundColor Cyan

$createBody = [Newtonsoft.Json.Linq.JObject]::Parse('{"description":"smoke test","public":false,"files":{"smoke-test.txt":{"content":"hello from PowerAutomate.Desktop.Modules smoke test"}}}')
$create = Invoke-Action -TypeName 'GistsCreateAction' -Inputs @{ Authentication = $authContext; Body = $createBody }
Write-Host ("  GistsCreate                                                  -> {0}  ({1} ms){2}" -f $create.Status, $create.Elapsed, $(if ($create.Error) { ": $($create.Error)" } else { '' })) -ForegroundColor $(if ($create.Status -eq 'OK') { 'Green' } else { 'Red' })
$chainResults = @($create)

if ($create.Status -eq 'OK') {
    $gistId = $create.Output.Id

    $get = Invoke-Action -TypeName 'GistsGetAction' -Inputs @{ Authentication = $authContext; GistId = $gistId }
    Write-Host ("  GistsGet ({0})              -> {1}  ({2} ms){3}" -f $gistId, $get.Status, $get.Elapsed, $(if ($get.Error) { ": $($get.Error)" } else { '' })) -ForegroundColor $(if ($get.Status -eq 'OK') { 'Green' } else { 'Red' })
    $chainResults += $get

    $updateBody = [Newtonsoft.Json.Linq.JObject]::Parse('{"description":"smoke test (updated)"}')
    $updateGist = Invoke-Action -TypeName 'GistsUpdateAction' -Inputs @{ Authentication = $authContext; GistId = $gistId; Body = $updateBody }
    Write-Host ("  GistsUpdate                                                  -> {0}  ({1} ms){2}" -f $updateGist.Status, $updateGist.Elapsed, $(if ($updateGist.Error) { ": $($updateGist.Error)" } else { '' })) -ForegroundColor $(if ($updateGist.Status -eq 'OK') { 'Green' } else { 'Red' })
    $chainResults += $updateGist

    $deleteGist = Invoke-Action -TypeName 'GistsDeleteAction' -Inputs @{ Authentication = $authContext; GistId = $gistId }
    Write-Host ("  GistsDelete                                                  -> {0}  ({1} ms){2}" -f $deleteGist.Status, $deleteGist.Elapsed, $(if ($deleteGist.Error) { ": $($deleteGist.Error)" } else { '' })) -ForegroundColor $(if ($deleteGist.Status -eq 'OK') { 'Green' } else { 'Red' })
    $chainResults += $deleteGist

    $getAfterDelete = Invoke-Action -TypeName 'GistsGetAction' -Inputs @{ Authentication = $authContext; GistId = $gistId }
    $expectedAfterDelete = 'FAIL'
    Write-Host ("  GistsGet (after delete; expect FAIL)                         -> {0}  ({1} ms){2}" -f $getAfterDelete.Status, $getAfterDelete.Elapsed, $(if ($getAfterDelete.Status -ne $expectedAfterDelete) { " (UNEXPECTED, expected $expectedAfterDelete)" } else { '' })) -ForegroundColor $(if ($getAfterDelete.Status -eq $expectedAfterDelete) { 'DarkGray' } else { 'Red' })
    $chainResults += $getAfterDelete
}

$global:GitHubSmokeResults = $results
$global:GitHubSmokeAuth = $authContext
$global:GitHubSmokeChainResults = $chainResults
