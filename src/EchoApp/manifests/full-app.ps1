kubectl delete ns echo-app-ns
kubectl delete pv echo-app-pv

k apply -f .\echo-app-pv.yaml

k create ns echo-app-ns
k apply -f .\echo-app-pvc.yaml -n echo-app-ns

k apply -f .\echo-app-config.yaml -n echo-app-ns
k apply -f .\echo-app-secret.yaml -n echo-app-ns

k apply -f .\echo-db-svc.yaml -n echo-app-ns
k apply -f .\echo-db-dep.yaml -n echo-app-ns

k apply -f .\echo-api-svc.yaml -n echo-app-ns
k apply -f .\final\echo-api-dep.yaml -n echo-app-ns

k apply -f .\echo-webapp-svc.yaml -n echo-app-ns
k apply -f .\final\echo-webapp-dep.yaml -n echo-app-ns