#!/bin/bash
cd ~
pwd

rm -fR Documents/workspace/microting/eform-angular-frontend/eform-client/src/app/plugins/modules/installationchecking-pn

cp -a Documents/workspace/microting/eform-angular-installationchecking-plugin/eform-client/src/app/plugins/modules/installationchecking-pn Documents/workspace/microting/eform-angular-frontend/eform-client/src/app/plugins/modules/installationchecking-pn

rm -fR Documents/workspace/microting/eform-angular-frontend/eFormAPI/Plugins/InstallationChecking.Pn

cp -a Documents/workspace/microting/eform-angular-installationchecking-plugin/eFormAPI/Plugins/InstallationChecking.Pn Documents/workspace/microting/eform-angular-frontend/eFormAPI/Plugins/InstallationChecking.Pn

# Test files rm
rm -fR Documents/workspace/microting/eform-angular-frontend/eform-client/e2e/Tests/installationchecking-settings
rm -fR Documents/workspace/microting/eform-angular-frontend/eform-client/e2e/Tests/installationchecking-general
rm -fR Documents/workspace/microting/eform-angular-frontend/eform-client/e2e/Page\ objects/InstallationChecking
rm -fR Documents/workspace/microting/eform-angular-frontend/eform-client/wdio-headless-plugin-step2.conf.js
rm -fR Documents/workspace/microting/eform-angular-frontend/eform-client/wdio-plugin-step2.conf.js
# Test files cp

cp -a Documents/workspace/microting/eform-angular-installationchecking-plugin/eform-client/e2e/Tests/installationchecking-settings Documents/workspace/microting/eform-angular-frontend/eform-client/e2e/Tests/installationchecking-settings
cp -a Documents/workspace/microting/eform-angular-installationchecking-plugin/eform-client/e2e/Tests/installationchecking-general Documents/workspace/microting/eform-angular-frontend/eform-client/e2e/Tests/installationchecking-general
cp -a Documents/workspace/microting/eform-angular-installationchecking-plugin/eform-client/e2e/Page\ objects/InstallationChecking Documents/workspace/microting/eform-angular-frontend/eform-client/e2e/Page\ objects/InstallationChecking
cp -a Documents/workspace/microting/eform-angular-installationchecking-plugin/eform-client/wdio-headless-plugin-step2.conf.js Documents/workspace/microting/eform-angular-frontend/eform-client/wdio-plugin-step2.conf.js
