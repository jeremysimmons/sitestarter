 var applicationPath = '';
 
 function handleError(msg, srcUrl, line){
 
       var receiveReq = createHttpRequest();
					
		var url = applicationPath + '/Admin/LogJavascriptError.aspx';
		
		var data  = "ErrorData=" + msg + "&SourcePage=" + srcUrl + "&Line=" + line;
		
		receiveReq.open("POST", url, true);
						 
		receiveReq.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
		receiveReq.setRequestHeader("Content-length", data.length);
		receiveReq.setRequestHeader("Connection", "close");
							 
		receiveReq.send(data);
}

window.onerror=handleError;
 