[CmdletBinding()]
param(
    [string]$branch
)

if ("$branch" -eq "") {
    $branch = $(git status -b --porcelain).Split("`n")[0].Substring(3)
}

docker image build `
 --build-arg BRANCH=$branch `
 -t "dak4net/slides:$branch" `
 -f ./docker/linux/Dockerfile .
 
docker container rm -f dak4net-slides
docker container run -d -P --name dak4net-slides "dak4net/slides:$branch"

$address = $(docker container port dak4net-slides 80).Replace('0.0.0.0', 'localhost')  
firefox "http://$address"