# Install flyctl
#pwsh -Command "iwr https://fly.io/install.ps1 -useb | iex"

fly launch --org endurabyte --name linkstand

fly certs create -a linkstand linkstand.net
fly certs create -a linkstand api.linkstand.net
