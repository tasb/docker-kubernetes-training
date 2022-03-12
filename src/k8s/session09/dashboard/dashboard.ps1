kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/v2.5.0/aio/deploy/recommended.yaml

#kubectl patch deployment kubernetes-dashboard -n kubernetes-dashboard --type 'json' -p '[{"op": "add", "path": "/spec/template/spec/containers/0/args/-", "value": "--enable-skip-login"}]'

kubectl apply -f sa.yaml

kubectl apply -f crb.yaml

$token = kubectl -n kubernetes-dashboard get secret $(kubectl -n kubernetes-dashboard get sa/admin-user -o jsonpath="{.secrets[0].name}") -o go-template="{{.data.token | base64decode}}"

Write-Host ""
Write-Host "Use this token on Kubernetes Dashboard"
Write-Host ""
Write-Host "--------------------------------------------------------------"
Write-Host ""
Write-Host "$token"
Write-Host ""
Write-Host "--------------------------------------------------------------"