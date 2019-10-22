# /bin/sh

# start Grafana and give it time to spin up
/usr/share/grafana/bin/grafana-server &
sleep 15

# create new user
curl -X POST -H "Content-Type: application/json" -d '{ 
    "name":"viewer", 
    "email":"viewer@org.com", 
    "login":"viewer",  
    "password":"readonly" 
}' http://admin:admin@localhost:3000/api/admin/users 

# set user's home dashboard   
curl -X PUT -H "Content-Type: application/json" -d '{ 
    "homeDashboardId":1
}' http://viewer:readonly@localhost:3000/api/user/preferences
