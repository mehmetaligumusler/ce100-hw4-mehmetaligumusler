@echo off

cls
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

cls
:enter_commit_message
echo Enter your commit message:
echo 1. Some Problems Fixed
echo 2. Custom message

set /p commit_choice=Enter your commit message selection (1/2): 

IF %commit_choice%==1 (
    set commit_message=Some Problems Fixed
) ELSE IF %commit_choice%==2 (
    set /p commit_message=Enter your custom commit message: 
) ELSE (
    echo Invalid input. Please enter a valid option.
    goto enter_commit_message
)

cls
echo Confirm your commit message: %commit_message%
set /p confirm_commit=Is this correct? (Y/N): 

IF /i "%confirm_commit%"=="Y" (
    goto continue
) ELSE IF /i "%confirm_commit%"=="N" (
    goto enter_commit_message
) ELSE (
    echo Invalid input. Please enter Y for Yes or N for No.
    goto enter_commit_message
)

:continue
"C:\Program Files\Git\bin\bash.exe" -c "git switch %branch_name%"
"C:\Program Files\Git\bin\bash.exe" -c "git add ."
"C:\Program Files\Git\bin\bash.exe" -c "git commit -m ""%commit_message%"""
"C:\Program Files\Git\bin\bash.exe" -c "git push -u origin %branch_name%"

cls
echo Finish Push!!!
pause
