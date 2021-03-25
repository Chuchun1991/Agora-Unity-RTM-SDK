#!/bin/bash
#============================================================================== 
#   build_all.sh :  build script for RTM SDK
#
#    This build script is the entry script that calls sublevel scripts.  Since 
#  the invoking environment is Mac, Windows build will require separate scripting
#  outside this script.  
#   
#  $1 Available build targets:
#	build or all - build the libraries (Windows will be a project zip)
#	clean  - clean up the projects
#	release - bundle the libraries* into Unity structure as samples and libs folders
#  $2 Path to a WinDll Zip file, used as input for release 
#  $3 Path to a Zip file as the final output zip file
#
#  * It will assume the Windows build is done in separate step and copied to
#	Projects/Windows/sdk.
#
#============================================================================== 

# CLEAN=yes

CURDIR=`pwd`
PROJDIR=$CURDIR/Projects
AgoraRTMSdk=$CURDIR/Release

function Build {
#--------------------------------------
# build for Android
#--------------------------------------
echo "build for android started..." 
cd $PROJDIR/Android/ && ./build.sh || exit 1
echo "build for android is done." 

#--------------------------------------
# build for iOS
#--------------------------------------
echo "build for ios started..." 
cd $PROJDIR/iOS/ && ./build.sh  || exit 1
echo "build for ios is done." 

#--------------------------------------
# build for Mac
#--------------------------------------
echo "build for mac started..."
cd $PROJDIR/macOS/ && ./build.sh || exit 1
echo "build for mac is done."

#--------------------------------------
# prepare for Windows
#    * build script will create a zip file of the VS project to run on a PC
#--------------------------------------
echo "prepare for Windows started..."
cd $PROJDIR/Windows/ && ./build.sh || exit 1
echo "build for Windows is done."
}

function Release {
#----------------------------------------
#release package
# $1 = input zip file for Windows
# $2 = output zip file for releasing
#----------------------------------------
echo "release package started..."
cd $CURDIR || exit 1
./release_package.sh $PROJDIR $AgoraRTMSdk $1
echo ">>> release package end"

#optional
./refresh_gitdemo.sh  $AgoraRTMSdk

echo "copy demo start..." 
cd $CURDIR || exit 1
./copy_demo.sh $AgoraRTMSdk $2
echo ">>> copy demo is done."
}

function Clean {
    rm -rf $AgoraRTMSdk/*
    cd $PROJDIR/Android/ && ./build.sh clean
    cd $PROJDIR/iOS/ && ./build.sh clean
    cd $PROJDIR/macOS/ && ./build.sh clean
    cd $PROJDIR/Windows/ && ./build.sh clean
    exit 0
}


# echo "clear data start"
#cd $CURDIR || exit 1
#./clear_data.sh $PROJDIR $AgoraRTMSdk
# echo "clear data end"
case "$1" in 
    "clean") echo "Clean"
	Clean
	     ;;
    "release") echo "Release"
	Release $2 $3
	     ;;
    "" | "all" | "build") echo "All"
	Build
	;;
    *) echo "invalide option to $0 [clean release all]"
	exit 1
	;;
esac

echo "All (Android, IOS, MAC) done under $AgoraRTMSdk"
# open $AgoraRTMSdk

exit 0
