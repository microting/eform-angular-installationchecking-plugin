#!/bin/bash

cd ~

if [ -d "Documents/workspace/microting/eform-angular-installationchecking-plugin/eform-client/src/app/plugins/modules/installationchecking-pn" ]; then
	rm -fR Documents/workspace/microting/eform-angular-installationchecking-plugin/eform-client/src/app/plugins/modules/installationchecking-pn
fi

cp -av Documents/workspace/microting/eform-angular-frontend/eform-client/src/app/plugins/modules/installationchecking-pn Documents/workspace/microting/eform-angular-installationchecking-plugin/eform-client/src/app/plugins/modules/installationchecking-pn

if [ -d "Documents/workspace/microting/eform-angular-installationchecking-plugin/eFormAPI/Plugins/InstallationChecking.Pn" ]; then
	rm -fR Documents/workspace/microting/eform-angular-installationchecking-plugin/eFormAPI/Plugins/InstallationChecking.Pn
fi

cp -av Documents/workspace/microting/eform-angular-frontend/eFormAPI/Plugins/InstallationChecking.Pn Documents/workspace/microting/eform-angular-installationchecking-plugin/eFormAPI/Plugins/InstallationChecking.Pn
