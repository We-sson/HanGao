$ErrorActionPreference = 'SilentlyContinue'
$events = Get-WinEvent -FilterHashtable @{LogName='Application'; StartTime=[datetime]'2026-10-30 15:00:00'; EndTime=[datetime]'2026-10-30 17:00:00'}
$hits = $events | Where-Object { $_.Message -match 'RobotSimulator|AnyCAD' }
"hits: $($hits.Count)"
foreach ($e in $hits) {
  $first = ($e.Message -split "`r?`n" | Select-Object -First 6) -join ' || '
  "{0}  [{1}] {2}  :: {3}" -f $e.TimeCreated, $e.Id, $e.ProviderName, $first
}
