# Routes

* Criar um novo servidor 
  *	/api/server
* Remover um servidor existente 
  *	/api/servers/{serverId}
* Recuperar um servidor existente 
  *	/api/servers/{serverId}
* Checar disponibilidade de um servidor 
  *	/api/servers/available/{serverId} 
* Listar todos os servidores 
  *	/api/servers
* Adicionar um novo vídeo à um servidor 
  *	/api/servers/{serverId}/videos
* Remover um vídeo existente OK ruim
  *	/api/servers/{serverId}/videos/{videoId}
* Recuperar dados cadastrais de um vídeo  ruim
  *	/api/servers/{serverId}/videos/{videoId}
* Download do conteúdo binário de um vídeo
  *	/api/servers/{serverId}/videos/{videoId}/binary
* Listar todos os vídeos de um servidor
  *	/api/servers/{serverId}/videos
* Reciclar vídeos antigos
  *	/api/recycler/process/{days}
  *	/api/recycler/status    







