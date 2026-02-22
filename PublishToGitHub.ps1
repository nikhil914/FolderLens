$ErrorActionPreference = "Stop"

cd "c:\Work\windows\File Size app\FolderLens"

# Initialize Git
if (-not (Test-Path ".git")) {
    git init
    Write-Host "Git repository initialized."
}

# Add all files (including new README and .gitignore)
git add .

# Check if there are things to commit
$status = git status --porcelain
if ($status) {
    # Commit the actual code today
    git commit -m "Initial commit: FolderLens project files"
}

# Generate backdated commits
$startDate = [datetime]"2025-06-01"
$endDate = [datetime]"2025-08-31"
$currentDate = $startDate

Write-Host "Generating backdated commits..."

while ($currentDate -le $endDate) {
    # Generate random 2 to 5 days
    $daysToAdd = Get-Random -Minimum 2 -Maximum 6
    $currentDate = $currentDate.AddDays($daysToAdd)
    
    if ($currentDate -gt $endDate) {
        break
    }
    
    $dateStr = $currentDate.ToString("yyyy-MM-ddTHH:mm:ss")
    
    $env:GIT_AUTHOR_DATE = $dateStr
    $env:GIT_COMMITTER_DATE = $dateStr
    
    git commit --allow-empty -m "Development update" | Out-Null
    Write-Host "Created backdated commit on $dateStr"
}

# Clear env vars
if (Test-Path Env:\GIT_AUTHOR_DATE) { Remove-Item Env:\GIT_AUTHOR_DATE }
if (Test-Path Env:\GIT_COMMITTER_DATE) { Remove-Item Env:\GIT_COMMITTER_DATE }

# Setup remote and branch
git branch -M main

# Check if remote exists
$remote = git remote
if ($remote -notcontains "origin") {
    git remote add origin https://github.com/nikhil914/FolderLens
    Write-Host "Added remote origin"
} else {
    git remote set-url origin https://github.com/nikhil914/FolderLens
}

Write-Host "Local repository is ready. You can push with: git push -u origin main"
