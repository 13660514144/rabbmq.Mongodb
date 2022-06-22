/*序列化表单,标准JSON字符*/
/*附件列表*/
var _EditOrBrow = 0;//全局变量，默认0，0代表是编辑表单，1：增加表单 2:浏览表单 3：LIST
var _CashFrmData;//缓存从服务器取的Form数据
var _CashListData;//缓存从服务器取的List(公共列表数据)数据
var _ParaMethod;//请求模式
var _servercode;//请求服务编码(读数据，初始化获取数据)
var _ReqServercode;//请求服务编码(向服务提交数据进行保存)
var _PcOrMbApp;//PC还是移动设备
var _VagueFlg='N';//是否模糊查询标记
var _ClassFlg = 'N';//是否查询下级
var _ListConFig = 'None';//公共列表字段集
var _BtnParaResult ;//按钮参数集合
var _SubListConFig;//如果有子表，此变量存储子表字段列集合，没有子表将会赋空字符
var _PagePath='None';//页面路径
var _ParentID;//当前选中行记录ID
var _PagePara;//分页参数
var _delayed = 50;//延时种子
function CallBackGetAttachList(data) {
    try {
        var DeBase64 = _ClientPack.Return_Base64(data);
        var obj = eval('(' + DeBase64 + ')');       
        if (obj.scuess == '0' && parseInt(obj.Page) > 0) {
            var TB = document.getElementById('Tb_Attach');
            var tr, td;
            for (var x = 0; x < obj.Data.length; x++) {
                tr = TB.insertRow();
                td = tr.insertCell();
                td.innerText = obj.Data[x].FILE_NAME;
                td = tr.insertCell();                
                if (_EditOrBrow == 0) {
                    td.innerHTML = "<span class=\"bt_span\" onclick=\"_Tb_Attach_Mothed._Del_Attach('" + obj.Data[x].FILE_PATH + "','" + obj.Data[x].ID + "')\">删除</span>";
                }                
                td.innerHTML += "&nbsp;&nbsp;&nbsp;&nbsp;<a class=\"bt_span\" target=\"_blank\" href=\"" + obj.Data[x].FILE_PATH + "\">下载</a>";
                td = tr.insertCell();
                td.style.display = 'none';
                td.innerText = obj.Data[x].FILE_PATH;
                td = tr.insertCell();
                td.style.display = 'none';
                td.innerText = obj.Data[x].ID;
            }
        }
    }
    catch (err) {
        _ErrMsg(err.message);
    }
}
function CallBackFillAttach(data) { 
    try{
        var DeBase64 = _ClientPack.Return_Base64(data);
        var obj = eval('(' + DeBase64 + ')');
        if (obj.scuess == '0') {
            var TB = document.getElementById('Tb_Attach');
            for (var x = 0; x < TB.rows.length; x++) {
                if (parseInt(obj.Data[0].ID) == parseInt(TB.rows[x].cells[3].innerText)) {
                    TB.deleteRow(x);
                    _ErrMsg('删除成功！！！');
                    break;
                }
            }
        }
    }
    catch (err) {
        _ErrMsg(err.message);
    }
}

var _Tb_Attach_Mothed = {
    _Del_Attach: function (Path, Id) {
        var JsonData = _ClientPack.Request_Filer64('{"ID":"' + Id + '","PathId":"' + Path + '"}');
        if (confirm('确认删除吗，删除后不可以恢复') == true) {
            _AsyncRequest._AsyncPost(JsonData, 'FillAttach', 'CallBackFillAttach', '/class/JsonReq.ashx', ModelCode, '', '');
        }
    }
    ,
    _GetAttach: function (TbId) {
        var obj = document.getElementById(TbId);
        var AttachJson = '';
        if (obj.rows.length > 0) {
            for (var x = 0; x < obj.rows.length; x++) {
                AttachJson += (AttachJson == '') ?
                '{"FileName":"' + obj.rows[x].cells[0].innerText + '","Path":"' + obj.rows[x].cells[2].innerText + '","NewID":"' + obj.rows[x].cells[3].innerText + '"}' :
                ',' + '{"FileName":"' + obj.rows[x].cells[0].innerText + '","Path":"' + obj.rows[x].cells[2].innerText + '"","NewID":"' + obj.rows[x].cells[3].innerText + '"}';
            }
            AttachJson = '{"Attach":[' + AttachJson + ']}';
        }
        else {
            AttachJson = '{"Attach":[]}';
        }
        return AttachJson;
    }
    ,
    _GetRp_Attach: function (TbId) {//编辑状态返回新增附件        
        var AttachJson = '{"Attach":[]}';
        try {
            var obj = document.getElementById(TbId);
            if (obj.rows.length > 0) {
                for (var x = 0; x < obj.rows.length; x++) {
                    if (obj.rows[x].cells[3].innerText == '') {
                        AttachJson += (AttachJson == '') ?
                '{"FileName":"' + obj.rows[x].cells[0].innerText + '","Path":"' + obj.rows[x].cells[2].innerText + '","NewID":"' + obj.rows[x].cells[3].innerText + '"}' :
                ',' + '{"FileName":"' + obj.rows[x].cells[0].innerText + '","Path":"' + obj.rows[x].cells[2].innerText + '","NewID":"' + obj.rows[x].cells[3].innerText + '"}';
                    }
                }
                AttachJson = '{"Attach":[' + AttachJson + ']}';
            }
            else {
                AttachJson = '{"Attach":[]}';
            }
        }
        catch (err) {
            AttachJson = '{"Attach":[]}';
        }
        return AttachJson;
    }
    ,
    _Search_Attach: function (IdKey) {
        var ID = document.getElementById(IdKey).value;
        var JsonData = _ClientPack.Request_Filer64('{"ID":"' + ID + '"}');
        _AsyncRequest._AsyncPost(JsonData, 'GetAttachList', 'CallBackGetAttachList', '/class/JsonReq.ashx', ModelCode, '', '');
    }
};
var _Str_Angement = {
    /*公共验证*/
    _GuestIdent : function()
    {
        var BoolIdent;
        var R_chg;
        try
        {
            for (var x = 0; x < FieldsIdent.length; x++) {
                var obj = FieldsIdent[x];
                var Mothed = obj.ChgMothed.split('*');
                for (var z = 0; z < Mothed.length; z++) {
                    if (Mothed[z] == '_isStrLong') {
                        R_chg = 'this.' + Mothed[z] + '("' + document.getElementById(obj.Field).value + '",' + obj.MaxLeng+');';
                    }
                    else {
                        R_chg = 'this.' + Mothed[z] + '("' + document.getElementById(obj.Field).value + '");';
                    }
                    BoolIdent = eval(R_chg);
                    if (BoolIdent.Flg == false) {
                        _ErrMsg(obj.FieldCn + '-->' + BoolIdent.Msg);
                        document.getElementById(obj.Field).focus();
                        break;
                    }
                }              
                if (BoolIdent.Flg == false) {
                    break;
                }
            }            
        }
        catch(err)
        {
            _ErrMsg(err.message);
            BoolIdent = { Flg: false,Msg:'无法检验数据' };
        }
        return BoolIdent.Flg;
    }
    ,
    _Rtrim: function (sInputString, iType) {
        var sTmpStr = ' ';
        var i = -1;

        if (iType == 0 || iType == 1) {
            while (sTmpStr == ' ') {
                ++i;
                sTmpStr = sInputString.substr(i, 1);
            }
            sInputString = sInputString.substring(i);
        }

        if (iType == 0 || iType == 2) {
            sTmpStr = ' ';
            i = sInputString.length;
            while (sTmpStr == ' ') {
                --i;
                sTmpStr = sInputString.substr(i, 1);
            }
            sInputString = sInputString.substring(0, i + 1);
        }
        return sInputString;
    }
    ,
    _StrRep: function (Str) {
        var Rstr = this._Rtrim(Str, 0);
        Rstr = Rstr.replace(/\'/g, "‘");
        Rstr = Rstr.replace(/\"/g, '“');
        Rstr = Rstr.replace(/\n/g, '');
        Rstr = Rstr.replace(/\r/g, '');
        return Rstr;
    }
    ,
    _isStrLong: function (str, len) {
        var L = this.getByteLen(str);
        if (L <= parseInt(len)) {
            return { Flg: true, Msg: '数据正确' };
        }
        else {
            return { Flg: false, Msg: '数据超长：' + (L - parseInt(len)) };
        }
    }
    ,
    /*计算字符长度*/
    getByteLen: function(str) {
        var len = 0;
        for (var i = 0; i < str.length; i++) {
            var a = str.charAt(i);
            if (a.match(/[^\x00-\xff]/ig) != null) {
                len += 2;
            } else {
                len += 1;
            }
        }
        return len;   
    } 
    ,
    _isNumber: function (str) {
        var ss = this._Rtrim(str, 0);
        var regu = /^[0-9]+$/;
        var re = new RegExp(regu);
        if (ss.search(re) != -1) {
            return { Flg: true,  Msg:'数据正确' };
        } else {
            return { Flg: false, Msg: '数字不规范' };
        }
    }
    ,
    _isNull: function (str) {//不可以空
        if (this._Rtrim(str, 0) == '') {
            return { Flg: false, Msg: '必填项不可以空' };
        }
        else {
            return { Flg: true, Msg: '数据正确' };
        }
    }
    ,
    _isInteger: function (str) {//纯数字
        var regu = /^[-]{0,1}[0-9]{1,}$/;
        var re = new RegExp(regu);
        if (re.test(this._Rtrim(str, 0))) {
            return { Flg: true, Msg: '数据正确' };;
        } else {
            return { Flg: false, Msg: '数字不规范' };
        }
    }
    ,
    _isDecimal: function (str) {//浮点数
        
        var re = /^\d+(?=\.{0,1}\d+$|$)/;
        if (!re.test(this._Rtrim(str, 0))) {
            return { Flg: false, Msg: '数字不规范' };
        }
        else {
            return { Flg: true, Msg: '数据正确' };
        }
        //var re = /^(\-|\+)?\d+(\.\d+)?$/;
        /*
        if (re.test(this._Rtrim(str, 0))) {
            if (RegExp.$1 == 0 && RegExp.$2 == 0) {
                return { Flg: false, Msg: '数字不规范' };
            }
            else {
                return { Flg: true, Msg: '数据正确' };
            }
        } else {
            return { Flg: false, Msg: '数字不规范' };
        }
        */
    }
    ,
    //| 日期有效性验证 
    //| 格式为：YYYY-MM-DD或YYYY/MM/DD  
    _IsValidDate:function(DateStr){ 
          var sDate=DateStr.replace(/(^\s+|\s+$)/g,'');//去两边空格; 
          if(sDate==''){ 
              return { Flg: false, Msg: '日期不规范' }; 
          } 
          //如果格式满足YYYY-(/)MM-(/)DD或YYYY-(/)M-(/)DD或YYYY-(/)M-(/)D或YYYY-(/)MM-(/)D就替换为'' 
          //数据库中，合法日期可以是:YYYY-MM/DD(2003-3/21),数据库会自动转换为YYYY-MM-DD格式 
          var s=sDate.replace(/[\d]{ 4,4 }[\-/]{1}[\d]{1,2}[\-/]{1}[\d]{1,2}/g,''); 
          if(s==''){//说明格式满足YYYY-MM-DD或YYYY-M-DD或YYYY-M-D或YYYY-MM-D 
            var t=new Date(sDate.replace(/\-/g,'/')); 
            var ar=sDate.split(/[-/:]/); 
            if(ar[0]!=t.getYear()||ar[1]!=t.getMonth()+1||ar[2]!=t.getDate()){//_ErrMsg('错误的日期格式！格式为：YYYY-MM-DD或YYYY/MM/DD。注意闰年。'); 
                return { Flg: false, Msg: '日期不规范' }; 
            } 
          }else{//_ErrMsg('错误的日期格式！格式为：YYYY-MM-DD或YYYY/MM/DD。注意闰年。'); 
              return { Flg: false, Msg: '日期不规范' }; 
          } 
        return  { Flg: true, Msg: '数据正确' };; 
    }     
    ,
    //计算两个日期的天数差
    _DateDiff:function(firstDate,secondDate){
        var firstDate = new Date(firstDate);
        var secondDate = new Date(secondDate);
        var diff = Math.abs(firstDate.getTime() - secondDate.getTime())
        var result = parseInt(diff / (1000 * 60 * 60 * 24));
        return result;
    }    
    ,
    _Standard: function (Pre, Fields) {//Pre为表名前置，有就传递，没有传空字符
        var FieldArr = Fields.split('|');
        var Flg = true;
        try {
            for (var x = 0; x < FieldArr.length; x++) {
                var ChkArr = FieldArr[x].split(',');
                var Values = this._Rtrim(document.getElementById(Pre + ChkArr[0]).value);
                var span = 'span_' + Pre + ChkArr[0];
                document.getElementById(span).innerHTML = '';
                var Run = 'this._' + ChkArr[3] + '("' + Values + '")';
                Flg = eval(Run);
                if (Flg == false) {
                    _ErrMsg('输入数据不符合规范性');
                    document.getElementById(Pre + ChkArr[0]).focus();
                    document.getElementById(span).innerHTML = '<span style="font-size:25px"><font color="red"><b>↖</b></font></span>';
                    break;
                }
            }
        }
        catch (err) {
        }
        return Flg;
    }
    ,
    /*是否JSON对象*/
    _IsJson: function (Data) {
        var Flg = false;
        if (typeof (Data) == "object" &&
            Object.prototype.toString.call(Data).toLowerCase() == "[object object]"
            && !Data.length) {
            Flg = true
        }
        return Flg;
    }
    ,
    _IsJsonNull: function (data) {
        for (var key in data) {
            return false;
        }
        return true;
    }
};
var _Tb_Serialize = {
    SetChkAll:function(TbId){
        var Tb = document.getElementById(TbId);
        if(Tb.rows[0].cells[0].getElementsByTagName("input")[0].checked == true)
        {
            for(var j=1;j<Tb.rows.length;j++)
            {
                Tb.rows[j].cells[0].getElementsByTagName("input")[0].checked=true;
            }
        }
        else
        {
            var RowsLen=Tb.rows.length-1;
            for(var j=RowsLen;j>0;j--)
            {
                Tb.rows[j].cells[0].getElementsByTagName("input")[0].checked=false;
            }            
        }
    }
    ,
    /*表单编辑子表封装*/
    SetTbMsg:  function (TbId, ColField, SubData) {
        var JsonTb = '';
        var ColArr = ColField.split(',');
        var Tb = document.getElementById(TbId);
        var Tr, Td;
        var col, cols;
        Tr=Tb.insertRow();
        Tr.style.backgroundColor='#f3f3f3';
        Td=Tr.insertCell();
        Td.innerHTML='<input  type="checkbox" name="_CHKS" onclick="_Tb_Serialize.SetChkAll(\''+TbId+'\')" />';        
        for(var x=1;x<ColArr.length;x++)
        {
            col = ColArr[x];
            cols = col.split(':');
            Td = Tr.insertCell();
            if (cols[1] == '1') {
                Td.style.display = 'none';
            }
            Td.innerText=ColArr[x].split(':')[2];
        }
        for(var j=0;j<SubData.length;j++)
        {
            Tr=Tb.insertRow();
            Td=Tr.insertCell();
            Td.innerHTML='<input  type="checkbox" name="_CHKS" value="'+SubData[j].ID+'" />';
            for(var x=1;x<ColArr.length;x++){
                col=ColArr[x];
                cols=col.split(':');
                var ev='SubData['+j+'].'+cols[0];
                Td=Tr.insertCell();
                if(cols[1]=='1')
                {
                    Td.style.display = 'none';
                    //Td.innerHTML='<input  type="text" name="'+cols[0]+'" placeholder="Please Input" class="tb-input" value="'+eval(ev)+'" />';
                }
                Td.innerText = eval(ev);
                /*
                else
                {
                    Td.innerText=eval(ev);
                }                
                */
            }
        }
    }
    ,
    /*表单浏览列表*/
    SetTbBrow:  function (TbId, ColField, SubData) {
        var ColArr = ColField.split(',');
        var Tb = document.getElementById(TbId);
        var Tr, Td;
        var col, cols;
        Tr=Tb.insertRow();
            Td=Tr.insertCell();
            Td.innerText='ID';        
        for(var x=1;x<ColArr.length;x++)
        {
            col = ColArr[x];
            cols = col.split(':');
            Td = Tr.insertCell();
            if (cols[1] == '1') {
                Td.style.display = 'none';
            }
            Td.innerText=ColArr[x].split(':')[2];
        }
        for(var j=0;j<SubData.length;j++)
        {
            Tr=Tb.insertRow();                      
            for(var x=1;x<ColArr.length;x++){
                col=ColArr[x];
                cols=col.split(':');
                var ev = 'SubData['+j+'].' + cols[0];
                Td = Tr.insertCell();
                if (cols[1] == '1') {
                    Td.style.display = 'none';
                }
                Td.innerText=eval(ev);                
            }
        }
    }    
    ,
    /*子表数据封装*/
    PackSubList:function(TbId, ColField)
    {
        var list = [];
        try {
            var JsonTb = '';
            var Tb = document.getElementById(TbId);
            var TbRow = Tb.rows.length;
            var ColValue;
            var col, cols;
            if (TbRow <= 1) {
                return list;
            }
            if (ColField == '') {
                return list;
            }
            var ColArr = ColField.split(',');
            for (var x = 1; x < TbRow; x++) {
                if (Tb.rows[x].cells[0].getElementsByTagName("input")[0].checked == true) {
                    col = ColArr[0].split(':')[0];
                    var ColJson = '"' + col + '":"' + Tb.rows[x].cells[0].getElementsByTagName("input")[0].value + '"';
                    /* 全部列取值 子表由对应表单增加和修改数据
                    for (var y = 1; y < ColArr.length; y++) {
                        col = ColArr[y].split(':');
                        if (Tb.rows[x].cells[y].getElementsByTagName("input").length > 0) {
                            ColValue = _Str_Angement._StrRep(Tb.rows[x].cells[y].getElementsByTagName("input")[0].value);
                            ColJson += (ColJson == '') ? '"' + col[0] + '":"' + ColValue + '"' :
                                ',"' + col[0] + '":"' + ColValue + '"';
                        }
                    }
                    */
                    for (var y = 1; y < ColArr.length; y++) {
                        col = ColArr[y].split(':');
                        ColValue = Tb.rows[x].cells[y].innerText;
                        ColJson += ',"' + col[0] + '":"' + ColValue;
                    }
                    JsonTb = JSON.parse('{' + ColJson + '}');
                    list.push(JsonTb);
                }
            }
            return list;
        }
        catch (err) {
            _ErrMsg(err.message);
            return list;
        }
    }
    ,    
    /*封装子表数据 不用，保留*/
    GetTbMsg: function (TbId, ColField, SubCode) {
        var JsonTb = '';
        var ColArr = ColField.split(',');
        var Tb = document.getElementById(TbId);
        var SubData=[];
        var TbRow = Tb.rows.length;
        var ColValue;
        if (TbRow <= 1) {            
            var jobj={
                ModelCode:SubCode,
                SubData:SubData                
            };
            return jobj;
        }

        for (var x = 1; x < TbRow; x++) { 
            if (Tb.rows[x].cells[0].getElementsByTagName("input")[0].checked == true) {
                var ColJson = '"' + ColArr[0] + '":"' + Tb.rows[x].cells[0].getElementsByTagName("input")[0].value + '"';                
                for (var y = 1; y < ColArr.length; y++) {
                    var col=cells[y].split(':');
                    if (Tb.rows[x].cells[y].getElementsByTagName("input").length > 0)
                    {
                        ColValue = _Str_Angement._StrRep(Tb.rows[x].cells[y].getElementsByTagName("input")[0].value);
                        ColJson += (ColJson == '') ? '"' + col[0] + '":"' + ColValue + '"' :
                        ',"' + col[0] + '":"' + ColValue + '"';                      
                    }
                    else
                    {
                        ColValue=Tb.rows[x].cells[y].innerText;
                        ColJson += (ColJson == '') ? '"' + col[0] + '":"' + ColValue + '"' :
                        ',"' + col[0] + '":"' + ColValue + '"';                          
                    }
                }
                SubData.push(JSON.parse('{' + ColJson + '}'));
            }
        }        
            var sobj={
                ModelCode:SubCode,
                SubData:SubData                
            };
        return sobj;
    }
    ,
    /*取一行记录，不用，保留*/
    GetTbFirstChk: function (TbId, ColField, SubCode) {//首列一定是主键ID，做CHECK选项框，新增行ID是空字符或0        
        var SubData=[];
        var ColArr = ColField.split(',');
        var Tb = document.getElementById(TbId);
        var TbRow = Tb.rows.length;
        if (TbRow <= 1) {
            return SubData;
        }

        for (var x = 1; x < TbRow; x++) {
            var ColJson = '';
            var ColValue;
            if (Tb.rows[x].cells[0].getElementsByTagName("input")[0].checked == true) {
                for (var y = 0; y < ColArr.length; y++) {                 
                    if(Tb.rows[x].cells[y].getElementsByTagName("input").length > 0)
                    {
                        ColValue=_Str_Angement._StrRep(Tb.rows[x].cells[y].getElementsByTagName("input")[0].value);
                        ColJson += (ColJson == '') ? '"' + ColArr[y] + '":"' + ColValue + '"' :
                        ',"' + ColArr[y] + '":"' + ColValue + '"';
                    }
                }
                SubData.push(JSON.parse('{' + ColJson + '}'));
                break;
            }
        }        
        return SubData;
    }
};
var _Form_Serialize = {
    /*初始HTTP参数*/
    GetHttpParas: function () {
        _ParaMethod = _GetUrlPara.getParam('_ParaMethod');
        _servercode = _GetUrlPara.getParam('_servercode');
        _EditOrBrow = _GetUrlPara.getParam('_EditOrBrow');
        _ParentID = _GetUrlPara.getParam('_ParentID');
        _PcOrMbApp = this.IsMobile();
    }
    ,
    IsMobile:function (){
        if (navigator.userAgent.match(/Android/i)
            || navigator.userAgent.match(/webOS/i)
            || navigator.userAgent.match(/iPhone/i)
            || navigator.userAgent.match(/iPad/i)
            || navigator.userAgent.match(/iPod/i)
            || navigator.userAgent.match(/BlackBerry/i)
            || navigator.userAgent.match(/Windows Phone/i)
        ) { return true; } else { return false; }
    }
    ,
    Chkradio:function(ChkObj)
    {
        var obj=document.getElementsByName(ChkObj);
        for(var x=0;x<obj.length;x++)
        {
            if(obj[x].checked==false)
            {
                obj[x].checked=false;
            }
        }
    }
    ,
    ChkCheckAll:function(ChkObj)
    {
        var obj=document.getElementsByName(ChkObj);
        for(var x=0;x<obj.length;x++)
        {
            if(obj[x].checked==false)
            {
                obj[x].checked=true;
            }
        }
    }    
    ,
    ChkCheckOnly:function(ChkObj)
    {
        var obj=document.getElementsByName(ChkObj);
        for(var x=0;x<obj.length;x++)
        {
            if(obj[x].checked==true)
            {
                obj[x].checked=true;
            }
            else
            {
                obj[x].checked=false;
            }
        }
    }    
    ,    
    SetFormData:function(data,flg)
    {
        /*0代表是编辑表单，1：增加表单 2: 浏览表单 3：LIST*/
        switch(flg)
        {
            case '2': //Brow
                for(var item in data)
                {
                    try
                    {
                        if (document.getElementById(item)) {
                            document.getElementById(item).innerText = data[item];
                        }
                    }
                    catch (err) {
                        _ErrMsg(err.message);
                    }
                }                 
                break;
            case '0': //EDIT 
                
                var Colarr = Win._ListConFig.split(',');
                var Col;
                var Inputtxt = document.getElementsByTagName("input");
                var Seltxt = document.getElementsByTagName("select");
                /*Text 赋值*/
                for (var item = 0; item < Inputtxt.length; item++) {
                    if (Inputtxt[item].type == 'text' || Inputtxt[item].type == 'password') {
                        Inputtxt[item].value = this._BindData(data, Inputtxt[item].id);
                    }
                }
                /*Select 赋值*/
                for (var item = 0; item < Seltxt.length; item++) {
                    var s = Seltxt[item];
                    for (var x = 0; x < Colarr.length; x++) {
                        Col = Colarr[x].split(':');
                        if (s.id == Col[0]) {
                            if (Col[5] == '1') {
                                GetConstKey(Col[6], s,data);
                                break;
                            }
                            if (Col[7] == '1') {
                                GetOutField(Col[8], s);
                                break;
                            }
                        }
                    }
                }

                break;
        }        
    }
    ,
    /*绑定TEXT数据*/
    _BindData: function (data, id) {
        var str = '';
        for (var item in data) {
            if (item == id) {
                str = data[item];
                break;
            }
        }
        return str;
    }
    /*绑定select 默认值*/
    ,
    _ChangeDefault: function (e, selvalue) {
        for (var x = 0; x < e.options.length; x++) {
            if (e.options[x].value == selvalue) {
                e.options[x].selected = true;
                break;
            }
        }
    }
    ,
    SetFormAdd: function () {
        var Colarr = Win._ListConFig.split(',');        
        var InputBtn = document.getElementsByTagName("select");
        for (var j = 0; j < InputBtn.length; j++) {
            var sel = InputBtn[j];
            for (var x = 0; x < Colarr.length; x++) {
                var Col = Colarr[x].split(':');
                try {
                    if (sel.id == Col[0]) {
                        if (Col[5] == '1') {//常量          
                            if (document.getElementById(Col[0])) {
                                GetConstKey(Col[6], sel);                                
                            }
                        }
                        if (Col[7] == '1') {//Sel外联表表字段多选     
                            
                            if (document.getElementById(Col[0])) {
                                GetOutField(Col[8], sel);           
                            }
                        }
                        break;
                    }
                }
                catch (err) {
                    _ErrMsg(err.message);
                }
            }
        }
       
    }
    ,
    /*锁定按钮状态，防止重复提交*/
    BtnStatus:function(BtnId,Flg)
    {
        try {
            document.getElementById(BtnId).disabled = Flg;
            if (Flg == true) {
                document.getElementById(BtnId).style.backgroundColor = '#cecece';
            }
            else {
                document.getElementById(BtnId).style.backgroundColor = '#617798';
            }
        }
        catch (err) { }
    }
    ,
    BtnObjStatus:function(BtnObj,Flg)
    {
        try {
            BtnObj.disabled = Flg;
            if (Flg == true) {
                BtnObj.style.backgroundColor = '#cecece';
            }
            else {
                BtnObj.style.backgroundColor = '#617798';
            }
        }
        catch (err) { }
    }
    ,
    /*取表单旧值*/
    GetOldFrmValue:function(OldName)
    {
        var Str='';
            for(var item in _CashFrmData[0])
            {
                try
                {
                    if (item.toUpperCase() == OldName.toUpperCase())
                    {
                        var OldValue = _CashFrmData[0][item];  
                        Str = OldValue;
                        break;
                    }
                }
                catch (err) {
                    _ErrMsg(err.message);
                }
            }            
        return Str;
    }
    ,
    /*从寄存的_CashFrmData对象取值，对比值无发生变化*/
    isExitsVar: function (variableName, NewValue) {
        var isflg = true;
        try {
            if(NewValue==this.GetOldFrmValue(variableName))
            {
                isflg = false;
            }
        } catch (err) {
            isflg = false;
        }
        return isflg;
    }
    ,
    JsonSerialize: function (Form) {
        var res = [], //存放结果的数组
        ChkArr = [], //多个复选结果
		current = null, //当前循环内的表单控件
		i, //表单NodeList的索引
		len, //表单NodeList的长度
		k, //select遍历索引
		optionLen, //select遍历索引
		option, //select循环体内option
		optionValue, //select的value
		form = Form; //用form变量拿到当前的表单，易于辨识
        len = form.elements.length;
        for (i = 0; i < len; i++) {

            current = form.elements[i];
            if (current.name != '__VIEWSTATE' && current.name != '__VIEWSTATEGENERATOR' && current.name != '__EVENTVALIDATION') {
                //disabled表示字段禁用，需要区分与readonly的区别
                if (current.disabled) continue;

                switch (current.type) {

                    //可忽略控件处理                               
                    case "file": //文件输入类型
                    case "submit": //提交按钮
                    case "button": //一般按钮
                    case "image": //图像形式的提交按钮
                    case "reset": //重置按钮
                    case undefined: //未定义
                        break;

                    //select控件                               
                    case "select-one":
                    case "select-multiple":
                        if (current.name && current.name.length) {
                            //console.log(current)
                            for (k = 0, optionLen = current.options.length; k < optionLen; k++) {

                                option = current.options[k];
                                optionValue = "";
                                if (option.selected) {
                                    if (option.hasAttribute) {
                                        optionValue = option.hasAttribute('value') ? option.value : option.text
                                    } else {
                                        //低版本IE需要使用特性 的specified属性，检测是否已规定某个属性
                                        optionValue = option.attributes('value').specified ? option.value : option.text;
                                    }
                                    //res.push(encodeURIComponent(current.name) + "=" + encodeURIComponent(optionValue));
                                    if (_Str_Angement._Rtrim(optionValue) != '') {
                                        res.push('"' + current.name + '":"' + _Str_Angement._StrRep(optionValue) + '"');
                                    }
                                }
                            }
                        }
                        break;
                    case "checkbox":  //复选框    
                        //这里有个取巧 的写法，这里的判断是跟下面的default相互对应。
                        //如果放在其他地方，则需要额外的判断取值
                        var chkflg = 0;
                        for (var x = 0; x < ChkArr.length; x++) {
                            if (ChkArr[x] == current.name) {
                                chkflg += 1;
                                break;
                            }
                        }
                        if (chkflg == 0 && current.name && current.name.length) {
                            var chkobj = document.getElementsByName(current.name);
                            var cLen = chkobj.length;
                            var rchk = '';
                            for (var k = 0; k < cLen; k++) {
                                if (chkobj[k].checked) {
                                    rchk += (rchk == '') ? _Str_Angement._Rtrim(chkobj[k].value) : ',' + _Str_Angement._Rtrim(chkobj[k].value);
                                }
                            }
                            if (rchk != '') {
                                res.push('"' + current.name + '":"' + rchk + '"');
                            }
                            ChkArr.push(current.name);
                        }
                        break;
                    //单选                           
                    case "radio":
                        if (!current.checked) break;

                    default:
                        //一般表单控件处理
                        if (current.name && current.name.length) {
                            //res.push(encodeURIComponent(current.name) + "=" + encodeURIComponent(current.value));
                            if (_Str_Angement._Rtrim(current.value) != '') {
                                res.push('"' + current.name + '":"' + _Str_Angement._StrRep(current.value) + '"');
                            }
                        }

                }
            }
        }
        var Jstr = '{' + res.join(",") + '}';        
        return JSON.parse(Jstr);
    }
    ,
    _EditSerialize: function (Form) {
        var res = [], //存放结果的数组
        ChangeData = [],
        ChkArr = [], //多个复选结果
		current = null, //当前循环内的表单控件
		i, //表单NodeList的索引
		len, //表单NodeList的长度
		k, //select遍历索引
		optionLen, //select遍历索引
		option, //select循环体内option
		optionValue, //select的value
		form = Form; //用form变量拿到当前的表单，易于辨识
        var Tmp_Field = ''; //编辑表单中,字段临时存储的值对象
        
        var Jstr;
        try
        {
            for (i = 0, len = form.elements.length; i < len; i++) {

                current = form.elements[i];
                if (current.name != '__VIEWSTATE' && current.name != '__VIEWSTATEGENERATOR' && current.name != '__EVENTVALIDATION') {
                    //disabled表示字段禁用，需要区分与readonly的区别
                    if (current.disabled) continue;

                    switch (current.type) {

                        //可忽略控件处理                                
                        case "file": //文件输入类型
                        case "submit": //提交按钮
                        case "button": //一般按钮
                        case "image": //图像形式的提交按钮
                        case "reset": //重置按钮
                        case undefined: //未定义
                            break;

                        //select控件                                
                        case "select-one":
                        case "select-multiple":
                            if (current.name && current.name.length) {
                                Tmp_Field = current.name;
                                for (k = 0, optionLen = current.options.length; k < optionLen; k++) {

                                    option = current.options[k];
                                    optionValue = "";
                                    if (option.selected) {
                                        if (option.hasAttribute) {
                                            optionValue = option.hasAttribute('value') ? option.value : option.text
                                        } else {
                                            optionValue = option.attributes('value').specified ? option.value : option.text;
                                        }
                                        //res.push(encodeURIComponent(current.name) + "=" + encodeURIComponent(optionValue));
                                        if (this.isExitsVar(Tmp_Field, _Str_Angement._Rtrim(optionValue)) == true) {                                                
                                            ChangeData.push('"' + current.name + '":"' + this.GetOldFrmValue(Tmp_Field) + '"');
                                            res.push('"' + current.name + '":"' + _Str_Angement._StrRep(optionValue) + '"');
                                        }
                                    }
                                }
                            }
                            break;
                        case "checkbox":  //复选框    
                            //这里有个取巧 的写法，这里的判断是跟下面的default相互对应。
                            //如果放在其他地方，则需要额外的判断取值
                            var chkflg = 0;
                            for (var x = 0; x < ChkArr.length; x++) {
                                if (ChkArr[x] == current.name) {
                                    chkflg += 1;
                                    break;
                                }
                            }
                            if (chkflg == 0 && current.name && current.name.length) {
                                var chkobj = document.getElementsByName(current.name);
                                var cLen = chkobj.length;
                                var rchk = '';
                                for (var k = 0; k < cLen; k++) {
                                    if (chkobj[k].checked) {
                                        rchk += (rchk == '') ? _Str_Angement._Rtrim(chkobj[k].value) : ',' + _Str_Angement._Rtrim(chkobj[k].value);
                                    }
                                }
                                if (rchk != '') {
                                    Tmp_Field =  current.name;
                                    if (this.isExitsVar(Tmp_Field,rchk) == true) {                                                
                                        ChangeData.push('"' + current.name + '":"' + this.GetOldFrmValue(Tmp_Field) + '"');
                                        res.push('"' + current.name + '":"' + rchk + '"');
                                    }
                                }
                                ChkArr.push(current.name);
                            }
                            break;
                        //单选                            
                        case "radio":
                            if (!current.checked) break;

                        default:
                            //一般表单控件处理
                            if (current.name && current.name.length) {                            
                                Tmp_Field = current.name;
                                //res.push(encodeURIComponent(current.name) + "=" + encodeURIComponent(current.value));
                                if (this.isExitsVar(Tmp_Field, _Str_Angement._Rtrim(current.value))==true) {                                        
                                    ChangeData.push('"' + current.name + '":"' + this.GetOldFrmValue(Tmp_Field) + '"');
                                    res.push('"' + current.name + '":"' + _Str_Angement._StrRep(current.value) + '"');
                                }
                                /**表单控件对象全部取值 页而包含*/
                                else if (current.name == 'ID') {
                                    res.push('"' + current.name + '":"' + current.value + '"');
                                }
                                else if(current.name == 'Code'){
                                    res.push('"' + current.name + '":"' + current.value + '"');
                                }
                                else if(current.name=='UnitCode'){
                                    res.push('"' + current.name + '":"' + current.value + '"');
                                }
                                else if (current.name.length > 2 && current.name.substring(current.name.length - 3) == '_ID') {
                                    res.push('"' + current.name + '":"' + current.value + '"');
                                }
                                
                            }
                    }
                }
            }
        }
        
        catch (err) {
            _ErrMsg(err.message);
            var Estr = {
                NewValue: {},
                OldValue: {}
            };
            return Estr;
        }
        if (ChangeData.length > 0) {
            var NewValue = JSON.parse('{' + res.join(",") + '}');
            var OldValue = JSON.parse('{' + ChangeData.join(",") + '}');
            var Tstr = {
                NewValue: NewValue,
                OldValue: OldValue
            };
            return Tstr;
        }
        else {
            _ErrMsg('没有数据产生变化');
            var Nstr = {
                NewValue: {},
                OldValue: {}
            };
            return Nstr;
        }
    }
};
var _ClientPack={
    /*上传Base64前处理/和+号*/
    Request_Filer64 : function(Str){        
        var BaseStr,filerstr;
        var base = new this.Url_Base64();
        try
        {
            BaseStr=base.encode(Str);
            filerstr=BaseStr.replace(/\+/g, "-");
            filerstr=filerstr.replace(/\//g, "*");
        }
        catch(err)
        {
            filerstr='';
        }
        return filerstr;
    }
    ,
    /*只做完整BASE64编码*/
    Get_BaseStr64 : function(Str){    
        var BaseStr,filerstr;
        var base = new this.Url_Base64();
        try
        {
            BaseStr=base.encode(Str);            
        }
        catch(err)
        {
            BaseStr='';
        }
        return BaseStr;        
    }
    ,
    /*还原BASE64*/
    Return_Base64 : function(Str){
        var BaseStr,filerstr;
        var base = new this.Url_Base64();
        try
        {
            BaseStr=base.decode(Str);            
        }
        catch(err)
        {
            BaseStr='';
        }
        return BaseStr;               
    }
    ,
    /*base64*/    
    Url_Base64 : function() {

        
        _keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

        
        this.encode = function (input) {
            var output = "";
            var chr1, chr2, chr3, enc1, enc2, enc3, enc4;
            var i = 0;
            input = _utf8_encode(input);
            while (i < input.length) {
                chr1 = input.charCodeAt(i++);
                chr2 = input.charCodeAt(i++);
                chr3 = input.charCodeAt(i++);
                enc1 = chr1 >> 2;
                enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
                enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
                enc4 = chr3 & 63;
                if (isNaN(chr2)) {
                    enc3 = enc4 = 64;
                } else if (isNaN(chr3)) {
                    enc4 = 64;
                }
                output = output +
            _keyStr.charAt(enc1) + _keyStr.charAt(enc2) +
            _keyStr.charAt(enc3) + _keyStr.charAt(enc4);
            }
            return output;
        }

        // public method for decoding  
        this.decode = function (input) {
            var output = "";
            var chr1, chr2, chr3;
            var enc1, enc2, enc3, enc4;
            var i = 0;
            input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");
            while (i < input.length) {
                enc1 = _keyStr.indexOf(input.charAt(i++));
                enc2 = _keyStr.indexOf(input.charAt(i++));
                enc3 = _keyStr.indexOf(input.charAt(i++));
                enc4 = _keyStr.indexOf(input.charAt(i++));
                chr1 = (enc1 << 2) | (enc2 >> 4);
                chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
                chr3 = ((enc3 & 3) << 6) | enc4;
                output = output + String.fromCharCode(chr1);
                if (enc3 != 64) {
                    output = output + String.fromCharCode(chr2);
                }
                if (enc4 != 64) {
                    output = output + String.fromCharCode(chr3);
                }
            }
            output = _utf8_decode(output);
            return output;
        }

        // private method for UTF-8 encoding  
        _utf8_encode = function (string) {
            string = string.replace(/\r\n/g, "\n");
            var utftext = "";
            for (var n = 0; n < string.length; n++) {
                var c = string.charCodeAt(n);
                if (c < 128) {
                    utftext += String.fromCharCode(c);
                } else if ((c > 127) && (c < 2048)) {
                    utftext += String.fromCharCode((c >> 6) | 192);
                    utftext += String.fromCharCode((c & 63) | 128);
                } else {
                    utftext += String.fromCharCode((c >> 12) | 224);
                    utftext += String.fromCharCode(((c >> 6) & 63) | 128);
                    utftext += String.fromCharCode((c & 63) | 128);
                }

            }
            return utftext;
        }

        // private method for UTF-8 decoding  
        _utf8_decode = function (utftext) {
            var string = "";
            var i = 0;
            var c = c1 = c2 = 0;
            while (i < utftext.length) {
                c = utftext.charCodeAt(i);
                if (c < 128) {
                    string += String.fromCharCode(c);
                    i++;
                } else if ((c > 191) && (c < 224)) {
                    c2 = utftext.charCodeAt(i + 1);
                    string += String.fromCharCode(((c & 31) << 6) | (c2 & 63));
                    i += 2;
                } else {
                    c2 = utftext.charCodeAt(i + 1);
                    c3 = utftext.charCodeAt(i + 2);
                    string += String.fromCharCode(((c & 15) << 12) | ((c2 & 63) << 6) | (c3 & 63));
                    i += 3;
                }
            }
            return string;
        }
    }        
    ,
    /*本地缓存*/
    SaveCache : function (LocalItem,jsonstr) {
        var Flg=true;
        try
        {
            if (jsonstr == '' || LocalItem=='') {                
                Flg=false;
            }
            else {
                var base = new this.Url_Base64();
                let storage = new Storage();
                storage.setItem({
                    name: LocalItem,
                    value: base.encode(jsonstr)
                });        
                //localStorage.setItem(LocalItem, base.encode(jsonstr));  
            }
        }
        catch(err)
        {
            _ErrMsg(err.message);
            Flg=false;
        }
        return Flg;
    }   
    ,
    /*获取本地缓存*/
    GetAgent : function (LocalItem) {
        var Agent;
        try {
            var base = new this.Url_Base64();
            let storage = new Storage();
            var s=storage.getItem(LocalItem);
            if (s != null) {
            //if (localStorage.getItem(LocalItem) != null) {
                //var jsonbase = localStorage.getItem(LocalItem);
                Agent = base.decode(s);
            }
            else {
                Agent = '';
            }
        }
        catch (err) {
            Agent = '';
        }
        return Agent;
    }    
    ,
    /*删除本地缓存*/
    DelLocalSave : function (LocalItem) {
        var Flg=true;
        try
        {            
            let storage = new Storage();
            storage.removeItem(LocalItem);
                //localStorage.removeItem(LocalItem);                      
        }
        catch(err)
        {
            _ErrMsg(err.message);
            Flg=false;
        }
        return Flg;
    }
    ,
    DelLocalAll : function () {
        var Flg=true;
        try
        {
            if (confirm("确定要清空本地数据吗？")) {
                //localStorage.removeItem(LocalItem);                      
                let storage = new Storage();
                storage.clear();
            }
            else
            {
                Flg=false;
            }
        }
        catch(err)
        {
            _ErrMsg(err.message);
            Flg=false;
        }
        return Flg;
    }    
};
var _GetUrlPara = {
    getParam : function(paramName) {
        paramValue = "", isFound = !1;
        if (location.search.indexOf("?") == 0 && location.search.indexOf("=") > 1) {
            arrSource = unescape(location.search).substring(1, location.search.length).split("&"), i = 0;
            while (i < arrSource.length && !isFound) arrSource[i].indexOf("=") > 0 && arrSource[i].split("=")[0].toLowerCase() == paramName.toLowerCase() && (paramValue = arrSource[i].split("=")[1], isFound = !0), i++
        }
        return paramValue == "" && (paramValue = null), paramValue
    }
};
// HTML导出Excel文件(兼容IE及所有浏览器)
var _CreatEccelOut = {
    ExportToExcel: function (tableid) {

        var filename;
        var obj = document.getElementById(tableid);

        try {
            filename =document.title+ '.xls';// obj.rows[0].cells[0].innerText + '.xls';
        }
        catch (err) {
            filename = '.xls';
        }
        filename = filename.replace(/\//g, "-");
        try {
            //fso = new ActiveXObject("Scripting.FileSystemObject");
            if (this.isIE() == true) {
                this.HtmlExportToExcelForIE(tableid, filename);
            }
            else {
                this.HtmlExportToExcelForEntire(tableid, filename);
            }
        }
        catch (err) {
            this.HtmlExportToExcelForEntire(tableid, filename);
        }
    }
    ,
    //IE浏览器导出Excel
    HtmlExportToExcelForIE: function (tableid, filename) {

        try {
            var html = "<html><head><meta charset='utf-8' />" + _style+"</head><body>" + document.getElementById(tableid).outerHTML + "</body></html>";
            var blob = new Blob([html], { type: "application/vnd.ms-excel" });
            window.navigator.msSaveOrOpenBlob(blob, filename);
        } catch (e) {
            _ErrMsg(e.description);
        }
    }
     ,
    //非IE浏览器导出Excel
    HtmlExportToExcelForEntire: function (obj, filename) {
        var blob, html;
        try {
            html = "<html><head><meta charset='utf-8' />" + _style+"</head><body>" + document.getElementById(obj).outerHTML + "</body></html>";
            // 实例化一个Blob对象，其构造函数的第一个参数是包含文件内容的数组，第二个参数是包含文件类型属性的对象
            blob = new Blob([html], { type: "application/vnd.ms-excel" });
            var a = document.getElementById("OutHomePageDlink"); // document.getElementsByTagName("a")[0];
            // 利用URL.createObjectURL()方法为a元素生成blob URL
            a.href = URL.createObjectURL(blob);
            // 设置文件名
            a.download = filename;
            a.click();
        }
        catch (err) {
            html = "<html><head><meta charset='utf-8' /></head><body>" + document.getElementById(obj).outerHTML + "</body></html>";
            blob = new Blob([html], { type: "application/vnd.ms-excel" });
            window.navigator.msSaveOrOpenBlob(blob, filename);
        }
    }
    ,
    HtmlExportToWord: function (obj) {
        var blob, html,name;
        name=document.title+'.doc';
        try {
            html = "<html><head><meta charset='utf-8' />" + _style+"</head><body>" + document.getElementById(obj).outerHTML + "</body></html>";
            //var bodyhtml=document.getElementsByTagName('html')[0].outerHTML; 
            // 实例化一个Blob对象，其构造函数的第一个参数是包含文件内容的数组，第二个参数是包含文件类型属性的对象
            blob = new Blob([html], { type: "application/vnd.ms-word" });
            var a = document.getElementById("OutHomePageDlink"); // document.getElementsByTagName("a")[0];
            // 利用URL.createObjectURL()方法为a元素生成blob URL
            a.href = URL.createObjectURL(blob);
            // 设置文件名
            a.download = name;
            a.click();
        }
        catch (err) {
            html = "<html><head><meta charset='utf-8' /></head><body>" + document.getElementById(obj).outerHTML + "</body></html>";
            blob = new Blob([html], { type: "application/vnd.ms-word" });
            window.navigator.msSaveOrOpenBlob(blob, filename);
        }
    }    
     ,
    isIE: function () {

        if (!!window.ActiveXObject || "ActiveXObject" in window) { return true; }

        else { return false; }

    }
};
document.write('<a id="OutHomePageDlink"></a>');
var _StoreExpires=24*60*60*1000;//默认24小时过期
class Storage{
        constructor(name){
            this.name = 'storage';
        }
        //设置缓存
        setItem(params){
            let obj = {
                name:'',
                value:'',
                expires:_StoreExpires,
                startTime:new Date().getTime()//记录何时将值存入缓存，毫秒级
            }
            let options = {};
            //将obj和传进来的params合并
            Object.assign(options,obj,params);
            if(options.expires){
            //如果options.expires设置了的话
            //以options.name为key，options为值放进去
                localStorage.removeItem(options.name);
                localStorage.setItem(options.name,JSON.stringify(options));
            }else{
            //如果options.expires没有设置，就判断一下value的类型
               	let type = Object.prototype.toString.call(options.value);
               	//如果value是对象或者数组对象的类型，就先用JSON.stringify转一下，再存进去
                if(Object.prototype.toString.call(options.value) == '[object Object]'){
                    options.value = JSON.stringify(options.value);
                }
                if(Object.prototype.toString.call(options.value) == '[object Array]'){
                    options.value = JSON.stringify(options.value);
                }
                localStorage.removeItem(options.name);
                localStorage.setItem(options.name,options.value);
            }
        }
        //拿到缓存
        getItem(name){
            let item = localStorage.getItem(name);
            //先将拿到的试着进行json转为对象的形式
            if(item !=null)
            {
                try{
                    item = JSON.parse(item);
                }catch(error){
                //如果不行就不是json的字符串，就直接返回
                    item = item;
                }

                //如果有startTime的值，说明设置了失效时间
                if(item.startTime){
                    let date = new Date().getTime();
                    //何时将值取出减去刚存入的时间，与item.expires比较，如果大于就是过期了，如果小于或等于就还没过期
                    if(date - item.startTime > item.expires){
                    //缓存过期，清除缓存，返回false
                        localStorage.removeItem(name);
                        return null;
                    }else{
                    //缓存未过期，返回值
                        return item.value;
                    }
                }else{
                //如果没有设置失效时间，直接返回值
                    return item;
                }
            }
            else
            {
                return null;
            }
        }
        //移出缓存
        removeItem(name){
            localStorage.removeItem(name);
        }
        //移出全部缓存
        clear(){
            localStorage.clear();
        }
    }
var _style = '<style>  .bootstrap-frm h1 {  	font: 25px "Helvetica Neue", Helvetica, Arial, sans-serif;  	padding: 0px 0px 10px 20px;  	display: block;  	border-bottom: 1px solid #DADADA;  	margin: -10px -30px 10px -30px;  	color: #888;  }  .bootstrap-frm h1>span {  	display: block;  	font-size: 16px;font-weight:bold;  }      .bootstrap-frm label {          font: 20px "Helvetica Neue", Helvetica, Arial, sans-serif;          /*display: block;*/          display: flex;          flex-direction: row;          align-items: center;          vertical-align: middle;          margin: 10px 0;          width: 33%;          float: left;      }    .bootstrap-frm label>span {      font: 20px "Helvetica Neue", Helvetica, Arial, sans-serif;  	float: left;  	width: 30%;  	text-align: right;  	padding-right: 10px;  	margin-top: 10px;  	color: #333;  	font-family: "Helvetica Neue", Helvetica, Arial, sans-serif;  	font-weight: bold;  }  .bootstrap-frm label>li {      list-style-type:none;      font: 20px "Helvetica Neue", Helvetica, Arial, sans-serif;  	float: left;  	width: 60%;  	text-align: left;  	padding-right: 10px;  	margin-top: 10px;  	color: #333;  	font-family: "Helvetica Neue", Helvetica, Arial, sans-serif;  }  #comment{      height:100%;width:100%;  }  form{      margin-top:30px;      border:1px solid;margin:0 auto;  }        table            {             margin-left:auto;              margin-right:auto;              margin-top:10px;              margin-bottom:30px;              max-width:100%;            border-collapse:separate;            border-spacing:0px 0px;            width:100%;            }          td{border:1px solid #ccc;             border-collapse:separate;             text-align:center;             vertical-align:middle;             padding:8px 3px;             }          .fontz{              color:#191970;              font-weight:bold;              font-size:20px;          }          .fontb{              color:#D2691E;              cursor:pointer;              font-size:18px;                      }             .tb-input {           padding: 9px;              border: solid 1px #E5E5E5;              outline: 0;              font: normal 15px/100% Verdana, Tahoma, sans-serif;              width: 80%;              background: #FFFFFF url("bg_form.png") left top repeat-x;              background: -webkit-gradient(linear, left top, left 25, from(#FFFFFF), color-stop(4%, #EEEEEE), to(#FFFFFF));              background: -moz-linear-gradient(top, #FFFFFF, #EEEEEE 1px, #FFFFFF 25px);              box-shadow: rgba(0,0,0, 0.1) 0px 0px 8px;              -moz-box-shadow: rgba(0,0,0, 0.1) 0px 0px 8px;              -webkit-box-shadow: rgba(0,0,0, 0.1) 0px 0px 8px;                      }          table tbody {              display: block;              height: 100%;              overflow-y: scroll;table-layout:fixed;              border-collapse:separate;              border-spacing:0px 0px;                      }              table thead ,          table tbody tr {              display: table;              width: 100%;              table-layout: fixed;               text-align: center;            border-collapse:separate;            border-spacing:0px 0px;                       }              thead th,          tbody td {              width: 50px;word-wrap:break-word;                         border-collapse:separate;             text-align:center;             vertical-align:middle;             padding:8px 3px;                     text-align: center;          }              table thead {              width: calc( 100% - 1em);          }</style>'; 