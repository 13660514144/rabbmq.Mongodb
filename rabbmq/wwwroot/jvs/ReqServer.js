/*公用异步提交，非阻断式*/
var _AsyncResult;
var _AsyncRequest = {
    _AsyncPost: function (Data, CallBackMothed, url,ifHeader,posthead,type) {
        try
        {
			switch(type)
			{
				case 'JSON' :
					this.requestURL_Post(url, JSON.stringify(Data), CallBackMothed,ifHeader,posthead,type);
					break;
				case 'FORM' :
					var formData = new FormData();
					for (var key in Data) {
						formData.append(key, Data[key]);     
					}				
					this.requestURL_Post(url, formData, CallBackMothed,ifHeader,posthead,type);
					break;	
				case 'FORMFILE' :	
					var frmData = new FormData();
					for (var key in Data) {
						if(key=='UpFile')
						{
							frmData.append('file', document.getElementById(Data[key]).files[0]);  
						}
						else
						{
							frmData.append(key, Data[key]);    
						}						
					}	
					this.requestURL_Post(url, frmData, CallBackMothed,ifHeader,posthead,type);
					break;					
			}
        }
        catch(err)
        {
            _ErrMsg(err.message);
        }
    }
    ,
    _AsyncGet: function (Data, CallBackMothed, url,ifHeader,posthead) {       
        this.requestURL_Get(url, Data, CallBackMothed,ifHeader,posthead);
    }
    ,
    requestURL_Get: function (urlstr, postdata, CallBackMothed,ifHeader,posthead) {
        try
        {
            var LXZ_xmlhttp = this._AsyncHttpRequest();
            if (LXZ_xmlhttp) {
                LXZ_xmlhttp.open('GET', urlstr, true);
                //LXZ_xmlhttp.setRequestHeader("X-Requested-With", "XMLHttpRequest");
                LXZ_xmlhttp.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
				LXZ_xmlhttp.setRequestHeader("Authorization", "Bearer ");
				if(ifHeader)
				{
					for(var item in posthead)
					{
						LXZ_xmlhttp.setRequestHeader(item, posthead[item]);
					}
				}				
                LXZ_xmlhttp.onreadystatechange = function () {
                    if (LXZ_xmlhttp.readyState == 4 && LXZ_xmlhttp.status == 200) {                        
                        if (CallBackMothed != '') {
							//console.log(LXZ_xmlhttp.responseText);
                            var json = JSON.parse(LXZ_xmlhttp.responseText);
                            var _Call = CallBackMothed + '(json)';                            
                            eval(_Call);                            
                        }
                    }
                }
                LXZ_xmlhttp.send(postdata);
            }
            else {
                _ErrMsg("您的浏览器不支持XMLHttpRequest ");
            }
        }
        catch(err)
        {
            _ErrMsg(err.message);
        }        
    }
    ,
    requestURL_Post: function (urlstr, postdata, CallBackMothed,ifHeader,posthead,type) {
        try
        {
            var LXZ_xmlhttp = this._AsyncHttpRequest();
            if (LXZ_xmlhttp) {
                LXZ_xmlhttp.open('POST', urlstr, true);
				switch(type)
				{
					case "JSON":
						LXZ_xmlhttp.setRequestHeader("Content-Type", "application/json");
						LXZ_xmlhttp.setRequestHeader("Authorization", "Bearer ");
						break;
					case "FORM":
						LXZ_xmlhttp.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
						LXZ_xmlhttp.setRequestHeader("Authorization", "Bearer ");
						break;	
					case "FORMFILE":
						//LXZ_xmlhttp.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
						//LXZ_xmlhttp.setRequestHeader('Content-Type', 'multipart/form-data');
						LXZ_xmlhttp.setRequestHeader("X-Requested-With", "XMLHttpRequest");
						LXZ_xmlhttp.setRequestHeader("Authorization", "Bearer ");
						break;													
				}
				if(ifHeader)
				{
					for(var item in posthead)
					{					
						LXZ_xmlhttp.setRequestHeader(item, posthead[item]);						
					}
				}				
                LXZ_xmlhttp.onreadystatechange = function () {
                    if (LXZ_xmlhttp.readyState == 4 && LXZ_xmlhttp.status == 200) {
                       
                        if (CallBackMothed != '') {         						
                            var json = JSON.parse(LXZ_xmlhttp.responseText);                               
                            var _Call = CallBackMothed + '(json)';                            
                            eval(_Call);                            
                        }
                    }
                }
				if(type=='FORMFILE')
				{
    /*
	//进度条标签  页面需要下列两项
    <progress value="0" max="100" id="progress" style="height: 20px; width: 100%;display:none;"></progress>
    <br />
    //div模拟进度条
    <div id="progressNumber" style="width: 0%; height: 40px; background-color: blue"></div>	
	*/					
					//LXZ_xmlhttp.upload.addEventListener('progress', handleProgress)
					LXZ_xmlhttp.upload.onprogress = function (evt) {
						if (evt.lengthComputable) {
							var percentComplete = Math.round(evt.loaded * 100 / evt.total);
							document.getElementById('progress').value = percentComplete;
							document.getElementById('progressNumber').style.width = percentComplete + "%";
						}
					};					
				}				
                LXZ_xmlhttp.send(postdata);
            }
            else {
                _ErrMsg("您的浏览器不支持XMLHttpRequest ");
            }
        }
        catch(err)
        {
            _ErrMsg(err.message);
        }
    }
    ,
    _AsyncHttpRequest: function () {
        var xmlreq;
        _AsyncResult = "";
        if (window.ActiveXObject) {
            try {
                xmlreq = new ActiveXObject("Msxml2.XMLHTTP");
            } catch (e) {
                xmlreq = new ActiveXObject("Microsoft.XMLHTTP");
            }
        }
        else {
            xmlreq = new XMLHttpRequest();
        }
        return xmlreq;
    }
  
};
/*公用异步提交，非阻断式*/