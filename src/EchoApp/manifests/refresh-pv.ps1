k delete deploy echo-db-dep -n echo-app-ns
k delete pvc echo-app-pv-claim -n echo-app-ns
k delete pv echo-app-pv

k apply -f .\echo-app-pv.yaml
k apply -f .\echo-app-pvc.yaml -n echo-app-ns
k apply -f .\echo-db-dep.yaml -n echo-app-ns