cd source
git submodule update --init --recursive
nuget restore SylphyHorn.sln
msbuild SylphyHorn.sln /p:Configuration=Release /m /verbosity:normal /p:WarningLevel=0
cd ..
Rename-Item -Path "source/SylphyHorn/bin/Release" -NewName "SylphyHornEx"
Compress-Archive -Path "source/SylphyHorn/bin/SylphyHornEx" -DestinationPath SylphyHornEx.zip
