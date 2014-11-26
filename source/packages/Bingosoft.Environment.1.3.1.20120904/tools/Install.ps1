# Filename: Install.ps1
param($installPath, $toolsPath, $package, $project)
$ServerSource = 'http://nuget.bingosoft.net/nuget'
$CurrentPackageName = $package.Id
$CurrentVersion = $package.Version

# process
$currentProject = $project
$currentProjectName = $currentProject.ProjectName
$currentDllProjectName = ''
$currentWebProjectName = ''

function Check-Project-Type($currentProject){
	$returnValue = 'default'
    $currentProjectName = $currentProject.ProjectName
	
	try{
		if ($currentProjectName.EndsWith('.WebApp')){
			$currentDllProjectName = $currentProjectName.Remove($currentProjectName.LastIndexOf('.WebApp'))
			if (get-project $currentDllProjectName) {
				$returnValue = 'kissu'
				$returnValue
			}
		}else{
			$currentWebProjectName = $currentProjectName + '.WebApp'
			if (get-project $currentWebProjectName) {
				$returnValue = 'kissu'
				$returnValue
			}
		}
	}catch{
	}
	
	$returnValue
} 

if((Check-Project-Type $currentProject) -eq 'kissu') {
	if ($currentProjectName.EndsWith('.WebApp')){
		# clear dll Project Content in web project

		# install package to dll Project
		$currentDllProjectName = $currentProjectName.Remove($currentProjectName.LastIndexOf('.WebApp'))
		Install-Package $CurrentPackageName -Version $CurrentVersion -Project $currentDllProjectName -Source $ServerSource
	} else {
		# clear web Project Content in dll project
		$currentProject.ProjectItems.Item('App_Config').Delete()

		# install package to web Project
		$currentWebProjectName = $currentProjectName + '.WebApp'
		Install-Package $CurrentPackageName -Version $CurrentVersion -Project $currentWebProjectName -Source $ServerSource
	}
}

# end of script