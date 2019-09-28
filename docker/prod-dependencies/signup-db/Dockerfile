# escape=`
FROM dak4dotnet/sql-server:2017

ENV DATA_PATH="C:\data" `
    sa_password="DockerCon!!!"

VOLUME ${DATA_PATH}

WORKDIR C:\init
COPY .\docker\prod-dependencies\signup-db\Initialize-Database.ps1 .
ENTRYPOINT ["powershell", "./Initialize-Database.ps1"]