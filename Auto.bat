@echo off

echo Which branch would you like to switch to?
echo 1. main
echo 2. feat
echo 3. flow

set /p branch_choice=Enter your branch selection (1/2/3): 

IF %branch_choice%==1 (
    set branch_name=main
) ELSE IF %branch_choice%==2 (
    set branch_name=feat
) ELSE IF %branch_choice%==3 (
    set branch_name=flow
) ELSE (
    echo Invalid input. Please enter a valid option.
    exit /b
)

set /p commit_message=Enter your commit message: 

"C:\Program Files\Git\bin\bash.exe" -c "git switch %branch_name%"
"C:\Program Files\Git\bin\bash.exe" -c "git add ."
"C:\Program Files\Git\bin\bash.exe" -c "git commit -m ""%commit_message%"""
"C:\Program Files\Git\bin\bash.exe" -c "git push -u origin %branch_name%"

echo Finish Push!!!
pause
