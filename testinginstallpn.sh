#!/bin/bash
sed '/\/\/ INSERT ROUTES HERE/i {' src/app/plugins/plugins.routing.ts -i
sed '/\/\/ INSERT ROUTES HERE/i path: "installationchecking-pn",' src/app/plugins/plugins.routing.ts -i
sed '/\/\/ INSERT ROUTES HERE/i canActivate: [AuthGuard],' src/app/plugins/plugins.routing.ts -i
sed '/\/\/ INSERT ROUTES HERE/i loadChildren: "./modules/installationchecking-pn/installationchecking-pn.module#InstallationCheckingPnModule"' src/app/plugins/plugins.routing.ts -i
sed '/\/\/ INSERT ROUTES HERE/i },' src/app/plugins/plugins.routing.ts -i
