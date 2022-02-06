addEventListener('fetch', event => {
  event.respondWith(handleRequest(event.request))
})
//Branch for Nottingham Sherwood: 8a2d9772-268f-4d62-8291-f7bb28f10302
async function handleRequest() {
  //Get Data
  const branch = await SETTING.get('branch')
  const url = 'https://www.thegymgroup.com/BranchGymBusynessBlock/GetBusynessForBranch/?branchId='+branch+'&configurationId=0749f44e-4aa9-495d-a37a-d84a36d4b999'
  const response = await fetch(url)
  const results = await response.json()
  //Assign Variables
  const capacity = results['currentBranch']['capacity']
  const updated = results['currentBranch']['lastUpdated']
  const threshhold = results['currentBranch']['threshold']
  //Determine status color
  let color
  switch (threshhold){
    case 'lower':
      color = 326144
      break
    case 'middle':
      color = 16185856
      break
    default:
      color = 16384000
  }
  //Post webhook
  const webhook = await SETTING.get('webhook')
  const data = {
    "content": "",
    "embeds": [
      {
        "title": capacity,
        "color": color,
        "timestamp": updated,
      }
    ]
  }
  const init = {
    body: JSON.stringify(data),
    method: "PATCH",
    headers: {
      "content-type": "application/json;charset=UTF-8",
    },
  }
  await fetch(webhook, init)
  return new Response('')
}
addEventListener("scheduled", event => {
  event.waitUntil(handleRequest())
})
addEventListener("fetch", event => {
  return event.respondWith(handleRequest())
})