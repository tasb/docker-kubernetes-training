#!/bin/sh

kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/v2.5.0/aio/deploy/recommended.yaml

#kubectl patch deployment kubernetes-dashboard -n kubernetes-dashboard --type 'json' -p '[{"op": "add", "path": "/spec/template/spec/containers/0/args/-", "value": "--enable-skip-login"}]'

kubectl apply -f sa.yaml

kubectl apply -f crb.yaml

$token = kubectl -n kubernetes-dashboard get secret $(kubectl -n kubernetes-dashboard get sa/admin-user -o jsonpath="{.secrets[0].name}") -o go-template="{{.data.token | base64decode}}"

echo ""
echo "Use this token on Kubernetes Dashboard"
echo ""
echo "--------------------------------------------------------------"
echo ""
echo "$token"
echo ""
echo "--------------------------------------------------------------"