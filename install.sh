#!/bin/bash

if [ ! -d "/var/www/microting/eform-angular-installationchecking-plugin" ]; then
  cd /var/www/microting
  su ubuntu -c \
  "git clone https://github.com/microting/eform-angular-installationchecking-plugin.git -b stable"
fi

cd /var/www/microting/eform-angular-installationchecking-plugin
su ubuntu -c \
"dotnet restore eFormAPI/Plugins/InstallationChecking.Pn/InstallationChecking.Pn.sln"

echo "################## START GITVERSION ##################"
export GITVERSION=`git describe --abbrev=0 --tags | cut -d "v" -f 2`
echo $GITVERSION
echo "################## END GITVERSION ##################"
su ubuntu -c \
"dotnet publish eFormAPI/Plugins/InstallationChecking.Pn/InstallationChecking.Pn.sln -o out /p:Version=$GITVERSION --runtime linux-x64 --configuration Release"

if [ -d "/var/www/microting/eform-angular-frontend/eform-client/src/app/plugins/modules/installationchecking-pn" ]; then
	su ubuntu -c \
	"rm -fR /var/www/microting/eform-angular-frontend/eform-client/src/app/plugins/modules/installationchecking-pn"
fi

su ubuntu -c \
"cp -av /var/www/microting/eform-angular-installationchecking-plugin/eform-client/src/app/plugins/modules/installationchecking-pn /var/www/microting/eform-angular-frontend/eform-client/src/app/plugins/modules/installationchecking-pn"
su ubuntu -c \
"mkdir -p /var/www/microting/eform-angular-frontend/eFormAPI/eFormAPI.Web/out/Plugins/"

if [ -d "/var/www/microting/eform-angular-frontend/eFormAPI/eFormAPI.Web/out/Plugins/InstallationChecking" ]; then
	su ubuntu -c \
	"rm -fR /var/www/microting/eform-angular-frontend/eFormAPI/eFormAPI.Web/out/Plugins/InstallationChecking"
fi

su ubuntu -c \
"cp -av /var/www/microting/eform-angular-installationchecking-plugin/eFormAPI/Plugins/InstallationChecking.Pn/InstallationChecking.Pn/out /var/www/microting/eform-angular-frontend/eFormAPI/eFormAPI.Web/out/Plugins/InstallationChecking"


echo "Recompile angular"
cd /var/www/microting/eform-angular-frontend/eform-client
su ubuntu -c \
"/var/www/microting/eform-angular-installationchecking-plugin/testinginstallpn.sh"
su ubuntu -c \
"npm run build"
echo "Recompiling angular done"


