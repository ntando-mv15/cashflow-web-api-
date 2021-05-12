/*
********************************************************************************************
Synopsis: Sample .NET Core Multi Branch Pipeline
Version: v1.0.0
Date Created: 24-03-2021
********************************************************************************************
Contacts:
 - Lead: David Cupido
 - DevOps Engineer: <Sibusiso Makhubu - sibum@nedbank.co.za>
********************************************************************************************
Implementation Status:
    [-] Sample .Net Core Demo API 
        (x) Build
        (-) Unit Testing
			* Waiting on team to check in tests
		(-) Package Deploy to UCD
			* Currently pushing to dev environment only not master branch yet
        (-) Code Quality
		 	* Scanning all long living branches
        (-) Systems Integration Testing
			* To be included once the testing team is onboarded on DevOps toolchain
		(-) functionality Testing
			* To be included once the testing team is onboarded on DevOps toolchain
		(-) Regression testing
			* To be included once the testing team is onboarded on DevOps toolchain
        
********************************************************************************************
*/

// -----------------------------------------------------------------------------------------
//
// The main pipeline section
//
import java.text.SimpleDateFormat

def completedStages
def lastStage
def version = "1.0.0"
def DEPLOY_API = false
def sqScannerMsBuildHome = "D:/Jenkins/tools/sonarqube-4.8.0-netcoreapp3.0"

// -------------------------------------------------------------------------------------------
//
// Global variables
//
def workspace_Dir = "D:/Jenkins/workspace/dograds/server-ping-service"
def Test_Coverage = "D:/Jenkins/workspace/dograds/sample-core-web-api/test/CompanyApi.Tests/opencover.xml"
def VSTestResults = "D:/Jenkins/workspace/dograds/sample-core-web-api/test/CompanyApi.Tests/TestResults.trx"
// -------------------------------------------------------------------------------------------
//
// The main pipeline section
//

def getGitCommitHash = {
    lastCommitHash = sh(returnStdout: true, script: 'git rev-parse HEAD').trim()
    bat "echo Git Commit Hash resolved to: ${lastCommitHash}"
}

def getGitInfo = {
    GIT_AUTHOR = sh(returnStdout: true, script: 'git log -1 --pretty=format:\'%an\'').trim()
    GIT_AUTHOR_EMAIL = sh(returnStdout: true, script: 'git log -1 --pretty=format:\'%ae\'').trim()
    GIT_REPO_NAME = sh(returnStdout: true, script: 'basename -s .git `git config --get remote.origin.url`').trim()
    GIT_REPO_URL = sh(returnStdout: true, script: 'git config --get remote.origin.url').trim()
    GIT_COMMIT_MSG = sh(returnStdout: true, script: 'git log --oneline -1 --pretty=%B').trim()
    GIT_TAG = sh(returnStdout: true, script: 'git describe --always --tags').trim() 
}

def getLastCommitMessage = {

    // If it's a PR pull request, read the last TWO commit messages
    // The first is the PR pull message, the second is the actual commit
    if (env.BRANCH_NAME =~ /PR-.+/) {
        message = sh(returnStdout: true, script: 'git log -2 --pretty=%B').trim()
    } else {
        message = sh(returnStdout: true, script: 'git log -1 --pretty=%B').trim()
    }

    // Remove Special Apostrophes from Message
    message = message.replaceAll(/["']/,"")
}

def getGitAuthor = {
    def commit = sh(returnStdout: true, script: 'git rev-parse HEAD')
    author = sh(returnStdout: true, script: "git --no-pager show -s --format='%an' ${commit}").trim()
    echo "Git author resolved to: ${author}"
}

def setSonarBranchTarget = {

    TARGET = "develop"

    if (BRANCH_NAME == "master") {
      TARGET = ""
    }
    if (BRANCH_NAME == "develop") {
      TARGET = "master"
    }
	  if (BRANCH_NAME.startsWith("release/")) {
      TARGET = "master"
    }
	  if (BRANCH_NAME.startsWith("feature/")) {
      TARGET = "develop"
    }
    if (BRANCH_NAME.startsWith("hotfix/")) {
      TARGET = "master"
    }
    if (BRANCH_NAME.startsWith("bugfix/")) {
      TARGET = "develop"
    }

    SONAR_TARGET = "${TARGET}"
}

def populateGlobalVariables = {
    getLastCommitMessage()
    getGitCommitHash()
    getGitAuthor()   
    getGitInfo()
}

def pushToUCD (siteName="UCD PROD", componentName, componentApplication, baseDir, pushVersion) {
    step([$class: 'UCDeployPublisher',
        siteName: siteName,
        component: [
            $class: 'com.urbancode.jenkins.plugins.ucdeploy.VersionHelper$VersionBlock',
            componentName: componentName,
            componentApplication: componentApplication,
            delivery: [
                $class: 'com.urbancode.jenkins.plugins.ucdeploy.DeliveryHelper$Push',
                pushVersion: pushVersion,
                baseDir: baseDir,
                fileIncludePatterns: '**/*',
                fileExcludePatterns: '',
                pushProperties: 'jenkins.server=Local\njenkins.reviewed=false',
                pushDescription: 'Pushed from Jenkins'
            ]
        ]
    ])
}

def deployFromUCD (siteName="UCD PROD", componentApplication, environment, process, deployVersion) {
    step([$class: 'UCDeployPublisher',
        siteName: siteName,
        deploy: [
            $class: 'com.urbancode.jenkins.plugins.ucdeploy.DeployHelper$DeployBlock',
            deployApp: componentApplication,
            deployEnv: environment,
            deployProc: process,
            deployVersions: deployVersion,
            deployOnlyChanged: false,
				skipWait: true
        ]
    ])
}

pipeline {

    options {
        // Ensure each branch only has 1 job running at a time
        disableConcurrentBuilds()
        timestamps()
        timeout(time: 20, unit: 'MINUTES')
        buildDiscarder(logRotator(numToKeepStr: '10', artifactNumToKeepStr: '10'))
    }

    agent {
        node {
            // Builds will only execute on uwp-vs2019
            label "dotnetcore50"
            customWorkspace "${workspace_Dir}"
        }
    }

    environment {
        // OpenCover and VS Test Environment variables
        OpenCover = "D:/OpenCover"        
        // Nuget Environment variables
        Nuget = "D:/Nuget"
        TestRunner = "C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Enterprise\\Common7\\IDE"       
        Nuget_Proxy = "https://nexus.nednet.co.za/repository/nuget-group"
		Repo_Nuget_Proxy = "https://nexus.nednet.co.za/repository/repo-nuget-proxy"
        // SonarQube Environment variables
        SonarQube_Project_Key = "dograds-server-ping-service"
        SonarQube_Project_Name = "dograds-server-ping-service"
        SonarQube_Project_Exclusions = "**/*.json,**/*Test*,**/*.js,**/Base/**/*,**/RequestMobileOtp/**/*,**/SendMoneyToMobile/**/*,**/MockData/**/*,**/PolicySettings/**/*"
		DIST = "${workspace_Dir}"
        //UrbanCode Deploy Variables
        UCD_APP_NAME = "DOGRADS"
	    UCD_COMPONENT_NAME = "dograds-server-ping-service"
        // Bitbucket
        PROJECTKEY = "DOGRAD"
        // MSBuild Environment variables
        Project_Solution_Name = "WindowsServiceSample.sln"
        Project_Folder_Name = "${workspace_Dir}\\WindowsServiceSample\\bin\\Release\\netcoreapp3.1"
        Project_Solution_Folder = "${workspace_Dir}\\"
        // Email Addresses
        To_Email = "sibum@nedbank.co.za"
        From_Email = "devops@nedbank.co.za"
    }

    stages {
	
	    stage ("Pipeline init") {
			steps {
				script {												
					echo "BRANCH_NAME: ${BRANCH_NAME}"
					setSonarBranchTarget()
					echo "====> Stage Result ${STAGE_NAME}: ${currentBuild.currentResult}"            	
				}	
                script {
                      dir("${env.Project_Solution_Folder}src\\dist"){ 
                          deleteDir()
                    }
                }
			}
		}

         stage("SonarQube Init"){
            steps {
                script{                   
                    // Set-up the SonarQube environment, which is defined in Jenkins under 'Global Tools Configuration'
                    withSonarQubeEnv('SonarQube') {
                        bat "echo \"${SONAR_HOST_URL}\""
                        bat "dotnet ${sqScannerMsBuildHome}/SonarScanner.MSBuild.dll begin /k:\"${env.SonarQube_Project_Key}\" \
                            /n:\"${env.SonarQube_Project_Name}\" \
                            /v:\"${BUILD_NUMBER}\" \
                            /d:sonar.verbose=\"true\" \
                            /d:sonar.cs.opencover.reportsPaths='${Test_Coverage}' \
                            /d:sonar.cs.vstest.reportsPaths='${VSTestResults}' \
                            /d:sonar.branch.name=\"${BRANCH_NAME}\" \
                            /d:sonar.host.url=\"${SONAR_HOST_URL}\" \
                            /d:sonar.login=\"${SONAR_AUTH_TOKEN}\" \
                            /d:sonar.exclusions=\"${env.SonarQube_Project_Exclusions}\" \
                            /d:sonar.buildbreaker.skip=\"true\""
                    }                    
                }
            }
         }

		stage ("Building Solution"){
            steps {
                script {
                    def MSBuild = tool 'MSBuild VS 2019'
                    bat "dotnet restore ${env.Project_Solution_Folder}${env.Project_Solution_Name} -s ${env.Nuget_Proxy}"
				    bat "Echo building solution"
                    bat "dotnet build ${env.Project_Solution_Folder}${env.Project_Solution_Name} -c Release"
                }
            }
        }

       stage("Publish Build"){
            steps{ 
                script {                 
                     dir("${env.Project_Folder_Name}"){ 
					   // added debug for qa
                       bat "dotnet publish ${env.Project_Solution_Folder}${env.Project_Solution_Name} -c Release --no-restore -o dist"             
                    }

                    dir("${env.DIST}"){ 
                        powershell(returnStdout: true, script: 'Get-ChildItem -Path . -Recurse -Force -Include appsettings.*.json -ErrorAction SilentlyContinue | remove-Item -recurse')                        
                    }            
                }      
            }
        }
        
         stage ("SonarQube End") {
            steps {
                script {                   
                    withSonarQubeEnv('SonarQube') {
                        // Complete the build process and upload the results to the SonarQube server for processing
                        bat "dotnet ${sqScannerMsBuildHome}/SonarScanner.MSBuild.dll end /d:sonar.login=${SONAR_AUTH_TOKEN}"
                    }                                       
                }
            }
        }

        stage ("UCD Publish") {
            when {
              anyOf {
                  branch "develop"
                  branch "master"
				  branch "release"
                  branch "devops"
              } 
            }
            steps {
                script{
                    try
                    {
					    if (BRANCH_NAME == "release")		
							{		
								pushToUCD("${UCD_COMPONENT_NAME}", "${UCD_APP_NAME}", "${DIST}", "release-Rel_${version}_${BUILD_NUMBER}")
							}else if (BRANCH_NAME != "release"){		
								pushToUCD("${UCD_COMPONENT_NAME}", "${UCD_APP_NAME}", "${DIST}", "${BRANCH_NAME}-Rel_${version}_${BUILD_NUMBER}")
							}							
                        
						if (BRANCH_NAME == "develop")		
							{		
								DEPLOY_API = true		
							}else if (BRANCH_NAME != "master"){		
								START_RELEASE = true		
							}
                    }
                    catch (Exception ex)
                    {
                        def error_message = ex.getMessage();

                        if (error_message.contains("Error processing command: Version with name ") && error_message.contains("already exists for Component")) {
                            echo "Component version exists already. Nothing will be created in UCD"
                        }
                        else {
                            echo "Error creating the UCD componenet. The error is as follows:"
                            echo ex.getMessage()
                            throw ex
                        }
                    }
                }
             }
        }

    }

    post {
        success {
            // Email result to the Development Team
            emailext attachLog: true, body: "Result: SUCCESS<br> URL: ${BUILD_URL}<br><br> Nice One! :-)", subject: "SUCCESS Jenkins: Build #${BUILD_NUMBER} for ${JOB_NAME}", from: "${env.From_Email}", to: "${env.To_Email}"
        }

        failure {
            // Email result to the Development Team
            emailext attachLog: true, body: "Result: FAILED<br> URL: ${BUILD_URL}<br><br> Lets Try Again! :-(", subject: "FAILED Jenkins: Build #${BUILD_NUMBER} for ${JOB_NAME}", from: "${env.From_Email}", to: "${env.To_Email}"
        }

        
    }
}