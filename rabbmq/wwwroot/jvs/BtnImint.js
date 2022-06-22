var _CurrentBtn;
var InitFlg='';
function  BtnObjStatus(BtnObj,Flg)
    {
        try {
            BtnObj.disabled = Flg;
            /*if (Flg == true) {
                BtnObj.style.backgroundColor = '#cecece';
            }
            else {
                BtnObj.style.backgroundColor = '#617798';
            }*/
        }
        catch (err) { }
    }
function _BtnVisable(event)
{
    var e = window.event;
    var obj = e.srcElement;   
    _CurrentBtn=obj;
    _PopMsg();//遮罩层
    BtnObjStatus(obj,true);//禁用按钮
    setTimeout(function(){
        var Obtn=obj.id;
		var Ev;
		if(InitFlg=='')
		{
            Ev=Obtn+'_Click()';
		}
		else
		{
			Ev=InitFlg+'.'+Obtn+'()';
		}
        eval(Ev);
        BtnObjStatus(obj, false);
    },100);
} 
var Frm_Imint={
    _BtnImint:function(){
        var InputBtn = document.getElementsByTagName("button");               
        for(var item=0 ;item<InputBtn.length;item++)
        {
            //if(InputBtn[item].type=='button')
            //{             
            //InputBtn[item].className ='set_5_button';
                InputBtn[item].addEventListener("click", function(e){                        
                    _BtnVisable();
                }, false);
            //}
                
        }         
    }
    ,
    _BtnEventHead: function (Id, Str) {
        if (document.getElementById('ScoBtn')) {
            var Btn = document.getElementById('ScoBtn');
            var InputBtn = document.createElement('button');
            InputBtn.id = Id;
            InputBtn.innerText = Str;
            InputBtn.className = 'set_5_button';
            InputBtn.addEventListener("click", function (e) {
                _BtnVisable();
            }, false);
            Btn.appendChild(InputBtn);
        }
        else {
            var Btn = document.getElementById('btnhead');
            var InputBtn = document.createElement('button');
            InputBtn.id = Id;
            InputBtn.innerText = Str;
            InputBtn.className = 'set_5_button';
            InputBtn.addEventListener("click", function (e) {
                _BtnVisable();
            }, false);
            Btn.appendChild(InputBtn);
        }
    }    
    ,
    _BtnEventFoot: function (Id, Str) {
        var Btn = document.getElementById('footer');
        var InputBtn = document.createElement('button');
        InputBtn.id = Id;
        InputBtn.innerText = Str;
        InputBtn.className = 'set_5_button';
        InputBtn.addEventListener("click", function (e) {
            _BtnVisable();
        }, false);
        Btn.appendChild(InputBtn);
    }    
};
_PageStart = 1;