var _PageHeight;
var _PageWidth;
var _OutSel;//外联表值集合寄存变量
var _DomainConstKey = 'DomainConstKey';
var _DomainUnitKey = 'DomainUnitKey';
var _DomainRoleKey = 'DomainRoleKey';
var _radomvar = Math.round(Math.random() * 10000); //js文件随机版本号，确保无缓存
var _CashRequestData;//缓存本地表单数据，做为编辑提交后刷新列表使用;
/*
ListFields 列表字段对
format: string=field:ishidden:fieldcn:ifsearch:datatype,
                field:ishidden:fieldcn:ifsearch:datatype
field：表字段定义
ishidden:是否隐藏 0否 1 是
feildcn:字段名称
ifsearch:是否查询 0否 1 是
datatype：数据类型 I:数字 F：带小数数字 N：字符 D:日期
 */
	function guid() {
        return 'xxxxxxxxxxxx4xxxyxxxxxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
            var r = Math.random()*16|0,
                v = c == 'x' ? r : (r&0x3|0x8);
            return v.toString(16);
        });
    }
function _PackClient(ParentData, SubList, ScheduleList, AttachList) {
    var domain = window.location.host;
    
    var user = GetPackUserKey();
    var json = {
        ParaMethod: _ParaMethod,
        servercode: _servercode,
        userkey: user,
        ParentData: ParentData,
        SubList: SubList,
        AttachList: AttachList,
    };
  
    return json;
}
function GetPackUserKey() {
    var user = _ClientPack.GetAgent(_domainuserkey);
    
    if (user != '') {
        var obj = JSON.parse(user);
        return obj;
    }
    else {
        var json = {
            User: "",
            UserCn: "",
            UnitCode: "",
            UnitCn: "",
            Role: "",
            UserType: "",
            Token: "",
            SysCode:""
        };
        return json;
    }
}
function SetPackUserKey(data) {
    _ClientPack.SaveCache(_domainuserkey, JSON.stringify(data));
}

/*
 * 外联表字段集合取值,同步方法，避免回写对象错误
 * 级联select选项，只需要监听上级控件来获取本级数据 
 *  
 */
function GetOutField(ReqPara, e) {
    setTimeout(function () {
        try {
            if (e.options.length <= 0) {
                var Req = ReqPara.split('*');
                var Listener = Req.length;
                if (Listener < 3) {
                    _SetRequestPara(Req[0], Req[1]);
                    PagingResult = GetPagePara();
                    PagingResult.PagingMode = 'None';
                    var PostStr = {};
                    var CallBackMothed = 'CallBackGetOutField';
                    var url = '/ident.ashx';
                    var s = {
                        ParentData: PostStr,
                        PagePara: PagingResult
                    };

                    var data = _PackClient(s, [], [], []);
                    _AsyncRequest._AsyncPostWback(data, CallBackMothed, url, e);
                }
                else {
                    var ListenerObj = document.getElementById(Req[2]);
                    ListenerObj.addEventListener('change',
                        function () { _ListenerSelLink(Req, ListenerObj, e); }, false);
                    if (_EditOrBrow == '0') {
                        _ListenerSelLink(Req, ListenerObj, e);
                    }
                }
            }
        }
        catch (err) {
            _ErrMsg(err.message);
        }
    }, 100);
}
/**
级联监听方法
 */
function _ListenerSelLink(Arr,selobj,e) {
    var SelValue = selobj.value;
    _SetRequestPara(Arr[0], Arr[1]);
    PagingResult = GetPagePara();
    PagingResult.PagingMode = 'None';
    var PostStr = { SelValue: SelValue};
    var CallBackMothed = 'CallBackGetOutField';
    var url = '/ident.ashx';
    var s = {
        ParentData: PostStr,
        PagePara: PagingResult
    };

    var data = _PackClient(s, [], [], []);
    _AsyncRequest._AsyncPostWback(data, CallBackMothed, url, e);
}
function CallBackGetOutField(data, e) {

    try {
        //_ErrMsg(data.Msg);
        _DelOptions(e);
        if (data.scuess && data.IsResult) {
            var Data = data.Result.Data;
            SetOption(e, '', '请选择');
            for (var x = 0; x < Data.length; x++) {
                SetOption(e, Data[x]['Code'], Data[x]['CodeCn']);
            }
            if (_EditOrBrow == '0') {
                var Field = e.id;
                var FieldResult = _CashFrmData[0];
                var value;
                for (var item in FieldResult) {
                    if (item == Field) {
                        value = FieldResult[item];
                        break;
                    }
                }
                for (var x = 0; x < e.options.length; x++) {
                    if (e.options[x].value == value) {
                        e.options[x].selected = true;
                        break;
                    }
                }
            }
        }
    }
    catch (err) {
        _ErrMsg(err.message);
    }
}
/*从本地存储获取常量值*/
function GetConstKey(code, e, data) {
    setTimeout(function () {
        try {
            var sel = e;//document.getElementById(selID);
            if (sel.options.length <= 0) {
                var base = new _ClientPack.Url_Base64();
                var jsonbase = localStorage.getItem('DomainConstKey');
                var ConstJson = base.decode(jsonbase);
                var obj = JSON.parse(ConstJson);
                var ConstLen = code.length;

                sel.options.length = 0;
                SetOption(sel, '', '请选择');
                var conter = 0;
                for (var x = 0; x < obj.length; x++) {
                    var Item = obj[x];
                    if (Item.Code.substr(0, ConstLen) == code && Item.Code.length > ConstLen) {
                        SetOption(sel, Item.Code, Item.CodeCn);
                        conter++;
                    }
                    else if (conter > 0) {
                        break;
                    }
                }
                /*编辑状态下需要赋初始值*/

                if (_EditOrBrow == '0') {
                    var Field = e.id;
                    var value;
                    for (var item in data) {
                        if (item == Field) {
                            value = data[item];
                            break;
                        }
                    }

                    for (var x = 0; x < e.options.length; x++) {
                        if (e.options[x].value == value) {
                            e.options[x].selected = true;
                            break;
                        }
                    }
                }
            }
        }
        catch (err) {
            _ErrMsg(err.message);
        }
    }, 100);
}
function SetOption(sel, objvalue, objtext) {
    sel.options.add(new Option(objtext, objvalue));
}
function _DelOptions(e) {    
    e.options.length = 0;
}
function SetChangeValue(Sel, TxtId) {
    document.getElementById(TxtId).value = Sel.options[Sel.selectedIndex].value;
    var Hidd = document.getElementById(TxtId + '_X');
    Hidd.value = Sel.options[Sel.selectedIndex].text;
    Sel.style.display = 'none';
    Hidd.style.display = '';
}
/*查询下拉框架匹配值并选中*/
function SelChageValue(selectedValue, selid) {
    var selectObj = document.getElementById(selid);
    var i = 0, flg = 0;
    for (i = 0; i < selectObj.length; i++) {
        if (selectObj.options[i].text.indexOf(selectedValue) > -1
            || selectObj.options[i].value == selectedValue) {
            selectObj.selectedIndex = i;
            flg = 1;
            break;
        }
    }
    if (flg == 0) {
        selectObj.selectedIndex = 0;
    }
}
/*大窗口居中默认宽高*/

/*编辑浏览窗口*/
function _CreatFrame(url,w,h) {
    _NewMask();
    var wd, wh;
    wh = Get_LXZ_WH();
    wd = Get_LXZ_WD();
    var ww = w ||  wd;
    var hh = h ||  wh;
    var ifrm = document.createElement("iframe");
    ifrm.id = '_OpenFullWin';
    ifrm.setAttribute("src", url);
    ifrm.setAttribute('frameBorder', 0);
    ifrm.style.position = 'absolute';
    ifrm.style.width = (ww + Get_scrollLeft() - 1) + 'px';
    ifrm.style.height = (hh + Get_scrollTop() - 1) + 'px';
    ifrm.style.left = '1px';
    ifrm.style.top = '1px';
    ifrm.style.zIndex = '5000';
    ifrm.style.background = 'white';
    ifrm.style.marginBottom = '0px';
    document.body.appendChild(ifrm);
}
/*自动比例Frame*/
function _CreateAutoWin(url, w, h) {
    _NewMask();
    var ifrm = document.createElement("iframe");
    var wd, wh;
    wh = Get_LXZ_WH();
    wd = Get_LXZ_WD();
    var ww = w || 1;
    var hh = h || 1;
    var MeW = parseInt((wd + Get_scrollLeft() - 1) * parseFloat(ww));
    var MeH = parseInt((wh + Get_scrollTop() - 1) * parseFloat(hh));
    ifrm.id = '_AuditWin';
    ifrm.style.position = 'absolute';
    ifrm.setAttribute('frameBorder', 0);
    ifrm.setAttribute("src", url);
    ifrm.style.width = MeW+'px';
    ifrm.style.height = MeH + 'px';
    ifrm.style.zIndex = '5001';
    ifrm.style.background = '#FFFFFf';
    document.body.appendChild(ifrm);
    _MoveCenter('_AuditWin', MeW, MeH, 3);
}
/*审核附件上传窗口*/
function _CreatAudit(url,w,h) {
    _NewMask();
    var ifrm = document.createElement("iframe");
    var ww = w || 800;
    var hh = h || 500;
    ifrm.id = '_AuditWin';
    ifrm.style.position = 'absolute';
    ifrm.setAttribute('frameBorder', 0);
    ifrm.setAttribute("src", url);
    ifrm.style.width = ww+'px';
    ifrm.style.height = hh+'px';
    ifrm.style.zIndex = '5001';
    ifrm.style.background = '#FFFFFf';
    document.body.appendChild(ifrm);
    _MoveCenter('_AuditWin', parseInt(ww), parseInt(hh), 3);
}
/*树型类编辑窗口*/
function _CreatTreeWin(url) {
    _NewMask();
    var ifrm = document.createElement("iframe");
    ifrm.id = '_AuditWin';
    ifrm.style.position = 'absolute';
    ifrm.setAttribute("src", url);
    ifrm.style.width = '500px';
    ifrm.style.height = '300px';
    ifrm.style.zIndex = '5002';
    ifrm.style.background = '#FFFFFf';
    document.body.appendChild(ifrm);
    _MoveCenter('_AuditWin', 500, 300, 3);
}
/*这遮罩*/
function _NewMask() {
    _closeiframe();
    var newMask = document.createElement("div");
    newMask.id = '_NewMask';
    newMask.style.position = "absolute";
    newMask.style.zIndex = "4999";
    _scrollWidth = Get_scrollLeft() + Get_LXZ_WD();
    _scrollHeight = document.body.scrollHeight + (30);// Get_scrollTop() + Get_LXZ_WH();
    newMask.style.border = '0px';
    newMask.style.width = _scrollWidth + "px";
    newMask.style.height = _scrollHeight + "px";
    newMask.style.top = "0px";
    newMask.style.left = "0px";
    newMask.style.background = "black";
    newMask.style.filter = "alpha(opacity=8)";
    newMask.style.opacity = "0.10";
    document.body.appendChild(newMask);
}
function _PopMsg() {
    _NewMask();
    var P = document.getElementById('_NewMask');
    var newSpsn = document.createElement("span");
    newSpsn.id = '_NewSpan';
    newSpsn.className = 'onlyload';
    //newSpsn.innerText = '数据加载中，请稍候......';
    newSpsn.innerHTML = '<img src="Img/load.gif"  alt="" style="margin-top:30px;" />';
    document.body.appendChild(newSpsn);
    /*setTimeout(function () {
        _closeiframe();        
    }, 10000);	*/
}
function _ErrMsg(Msg) {
    _NewMask();
    var P = document.getElementById('_NewMask');
    var newSpsn = document.createElement("div");
    newSpsn.id = '_NewSpan';
    newSpsn.className = 'popmsg';
    newSpsn.innerHTML = '<div class="errtitle"><div class="clstitle" onclick="_closeiframe()">X</div>' +
        '</div><span class="spanmsg">' + Msg + '</span>';
    document.body.appendChild(newSpsn);
    
    setTimeout(function () {
        _closeiframe();        
    }, 5000);
}
/*config确认窗口回调 标记 */
var _ConFigCallBackFlg = false;
function _ConfigMsg(Msg,CallBack) {
    _NewMask();
    var newSpsn = document.createElement("div");
    newSpsn.id = '_NewSpan';
    newSpsn.className = 'configmsg';
    newSpsn.innerHTML = '<div class="errtitle"><div class="clstitle" ' +
        'onclick="_closeiframe()">X</div>' +
        '</div><span class="spanmsg">' + Msg + '</span>';
    newSpsn.innerHTML += '<div class="configfooter">' +
        '<button class="set_5_button" onclick="eval(' + CallBack + ')">是</button >' +
        '<button class="set_5_button" ' +
        'onclick="_closeiframe()">否</button>' +
        '</div > ';    
    document.body.appendChild(newSpsn);
}
/*Iframe使用，非父级使用 */
function _TitleClsWin() {
    //parent._Form_Serialize.BtnObjStatus(parent._CurrentBtn, false);
    parent._closeiframe();
}
function _closeiframe() {
    try {
        var iframe = document.getElementsByTagName("iframe");
        var div = document.getElementById('_NewMask');
        document.body.removeChild(div);
        for (var x = 0; x < iframe.length; x++) {
            if (iframe[x]) {
                document.body.removeChild(iframe[x]);
            }
        }
        var span = document.getElementById('_NewSpan');
        document.body.removeChild(span);
    }
    catch (err) {

    }
}
function Get_LXZ_WH() {  //可视区高度
    return (document.documentElement.clientHeight == 0) ? document.body.clientHeight : document.documentElement.clientHeight;
}
function Get_LXZ_WD() {  //可视区宽度
    return (document.documentElement.clientWidth == 0) ? document.body.clientWidth : document.documentElement.clientWidth;
}
function Get_LXZ_SCREENH() {  //分辨率高度
    return window.screen.height;
}
function Get_LXZ_SCREEND() {  //分辨率宽度
    return window.screen.width;
}

/*屏幕滚动宽高*/
function Get_scrollTop() {
    return Math.max(document.documentElement.scrollTop, document.body.scrollTop);
}
function Get_scrollLeft() {
    return Math.max(document.documentElement.scrollLeft, document.body.scrollLeft);
}
/*屏幕滚动宽高*/
/*滚动一个层居中*/
function _MoveCenter(objid, objw, objh, flg) {
    //flg=1 左右居中  //FLG=2上下居中  //FLG=3 上下左右居中
    switch (flg) {
        case 1:
            document.getElementById(objid).style.left = Get_scrollLeft() + (Get_LXZ_WD() - objw) / 2 + 'px';
            break;
        case 2:
            document.getElementById(objid).style.top = Get_scrollTop() + (Get_LXZ_WH() - objh) / 2 + 'px';
            break;
        case 3:
            document.getElementById(objid).style.top = Get_scrollTop() + (Get_LXZ_WH() - objh) / 2 + 'px';
            document.getElementById(objid).style.left = Get_scrollLeft() + (Get_LXZ_WD() - objw) / 2 + 'px';
            break;
    }
}
/*滚动一个层居中*/

/*token续期*/
var _domainuserkey = 'domainuserkey';
function _RenewalToken(UserKey) {
    if (UserKey.Token == '' || UserKey.User == '') {
        let storage = new Storage();
        storage.removeItem(_domainuserkey);
    }
    else {
        _ClientPack.SaveCache(_domainuserkey, JSON.stringify(UserKey));
    }
}
/*自定义JS文件装载，在HTML LOAD 完成后*/
function _LoadBtnJs(path) {
    var file = path || "NO";
    var script, Url;
    if (_PagePath != 'None') {
        if (file == 'NO') {
            var Dir = _PagePath.split('/');
            var JsPath = '/EventJs/' + Dir[Dir.length - 3] + '/' + Dir[Dir.length - 2] + '/';
            var JsFile = Dir[Dir.length - 2] + '.js?v=' + _radomvar;
            Url = JsPath + JsFile;
            script = document.createElement('script');
            script.type = 'text/javascript';
            script.src = Url;
            document.getElementsByTagName('head')[0].appendChild(script);
            
        }
        else {
            script = document.createElement('script');
            script.type = 'text/javascript';
            script.src = file;
            document.getElementsByTagName('head')[0].appendChild(script);
        }
    }
    else {
        if (file == 'NO') {
            setTimeout('_LoadBtnJs()', 1000);
        }
        else {
            setTimeout('_LoadBtnJs("' + file+'")', 1000);
        }
    }
}
/*刷新LIST */
function _FrmRfresh(Flg,data) {
    switch (Flg) {
        case '0': //EDIT   当前页查询20201118修正：当前表单数据刷新列表，不请求服务器
            Win._RunListMothed('C');            
            //_tb_elements._SetColvalue('SubList', data); 用此法常量项及外联表项无法刷新
            break;
        case '1'://add  首页查询
            Win._RunListMothed('T');
            break;
    }
    _TitleClsWin();
}

/*按钮事件绑定页面及参数 通用ADD,EDIT,BROW 公共列表LIST绑定*/
function _BindingBtnClick(ClickID,IfPath) {
    var ID;
    var Ev;
    var Rs;
    IfPath = (IfPath === undefined) ? true : false;
    for (var x = 0; x < _BtnParaResult.length; x++) {
        var obj = _BtnParaResult[x];
        if (obj.BtnOwerId == ClickID) {
            if (obj.IfGetId == '000001') {
                ID = _tb_elements._GetColvalue('SubList', 'ID');
                if (ID == '') {
                    Ev = '';
                }
                else {
                    Ev = obj.WinMothed;
                    //Ev += '("' + _PagePath;
                    Ev += '("' + (IfPath == true ? _PagePath : '');
                    Rs = obj.ParasResult.split(',');
                    Ev += Rs[0];
                    Ev += '&_ParentID=' + ID + '"';
                    switch (Rs.length) {
                        case 1:
                            Ev += ')';
                            break;
                        case 2:
                            Ev += ',' + Rs[1] + ')';
                            break;
                        case 3:
                            Ev += ',' + Rs[1] + ',' + Rs[2] + ')';
                            break;
                    }
                }
            }
            else {
                ID = '0';
                Ev = obj.WinMothed;
                //Ev += '("' + _PagePath;
                Ev += '("' + (IfPath == true ? _PagePath : '');
                Rs = obj.ParasResult.split(',');                
                Ev += Rs[0];
                Ev += '&_ParentID=' + ID + '"';
                switch (Rs.length) {
                    case 1:
                        Ev += ')';
                        break;
                    case 2:
                        Ev += ',' + Rs[1] + ')';
                        break;
                    case 3:
                        Ev += ',' + Rs[1] + ',' + Rs[2] + ')';
                        break;
                }
            }
            break;
        }
    }
    return Ev;
}
/*树型结构都按钮事件 */
function _TreeBtnClick(ClickID,IfPath) {
    var ID;
    var Ev;
    var Rs;
    IfPath = (IfPath === undefined) ? true : false;
    for (var x = 0; x < _BtnParaResult.length; x++) {
        var obj = _BtnParaResult[x];
        if (obj.BtnOwerId == ClickID) {
            if (obj.IfGetId == '000001') {
                if (typeof (o) == "undefined") {
                    ID = "0";
                }
                else {
                    ID = o.id;
                }
                Ev = obj.WinMothed;
                //Ev += '("' + _PagePath;
                Ev += '("' + (IfPath == true ? _PagePath :'') ;
                Rs = obj.ParasResult.split(',');
                Ev += Rs[0];
                Ev += '&_ParentID=' + ID + '&v=' + Math.round(Math.random() * 10000) + '"';
                switch (Rs.length) {
                    case 1:
                        Ev += ')';
                        break;
                    case 2:
                        Ev += ',' + Rs[1] + ')';
                        break;
                    case 3:
                        Ev += ',' + Rs[1] + ',' + Rs[2] + ')';
                        break;
                }
            }
            else {
                ID = '0';
                Ev = obj.WinMothed;
                //Ev += '("' + _PagePath;
                Ev += '("' + (IfPath == true ? _PagePath : '');
                Rs = obj.ParasResult.split(',');
                Ev += Rs[0];
                Ev += '&_ParentID=' + ID + '&v=' + Math.round(Math.random() * 10000) + '"';
                switch (Rs.length) {
                    case 1:
                        Ev += ')';
                        break;
                    case 2:
                        Ev += ',' + Rs[1] + ')';
                        break;
                    case 3:
                        Ev += ',' + Rs[1] + ',' + Rs[2] + ')';
                        break;
                }
            }
            break;
        }        
    }
    return Ev;
}
/*设置提交参数 _ParaMethod/_servercode 分初始化(0)设置和数据提交(1)设置，无返回值 */
function _SetPageReqParas(Para, ClickID) {
    try {
        for (var x = 0; x < Win._BtnParaResult.length; x++) {
            var obj = Win._BtnParaResult[x];
            if (obj.BtnOwerId == ClickID) {
                switch (Para) {
                    case 0:
                        _ParaMethod = obj.ServiceType;
                        _servercode = obj.ServiceCode;
                        break;
                    case 1:                       
                        _ParaMethod = obj.ReqType;
                        _servercode = obj.ReqServercode;
                        break;
                }
                break;
            }
        }
    }
    catch (err) {
        for (var x = 0; x < _BtnParaResult.length; x++) {
            var obj = _BtnParaResult[x];
            if (obj.BtnOwerId == ClickID) {
                switch (Para) {
                    case 0:
                        _ParaMethod = obj.ServiceType;
                        _servercode = obj.ServiceCode;
                        break;
                    case 1:
                        _ParaMethod = obj.ReqType;
                        _servercode = obj.ReqServercode;
                        break;
                }
                break;
            }
        }
    }
}
/*设置提交参数，针对联表SELECT*/
function _SetRequestPara(Mothed, Code) {
    _ParaMethod = Mothed;
    _servercode = Code;
}

/*表单编辑、查看赋值*/
var _FormSetValue = {
    _FromRequestServer: function () {
        try {
            var BtnId = Win._CurrentBtn.id;
            _SetPageReqParas(0, BtnId);
            PagingResult = GetPagePara();
            PagingResult.PagingMode = 'None';
            //var PostStr = { ID: _ParentID };
            var PostStr = { ID: Win._tb_elements._GetColvalue('SubList','ID') };
            var CallBackMothed = 'CallBackPageLoad';
            var url = '/ident.ashx';
            var s = {
                ParentData: PostStr,
                PagePara: PagingResult
            };
            var data = _PackClient(s, [], [], []);
            _AsyncRequest._AsyncPost(data, CallBackMothed, url);
        }
        catch (err) {
            _ErrMsg(err.message);
        }
    }
    ,
    _FromRequestList: function () {
        try {
            var list = Win.document.getElementById('SubList');
            var chk = Win.document.getElementsByName('_CHKS');
            var row = 0;
            for (var x = 0; x < chk.length; x++) {
                if (chk[x].checked == true) {
                    row = x + 1;
                    break;
                }
            }
            var fields = Win._TbListField.split(',');
            var s = [];
            for (var x = 1; x < fields.length; x++) {
                var field = '"'+fields[x] + '":"' + list.rows[row].cells[x].innerText+'"';
                s.push(field);
            }
            var obj = '{' + s.join(",") + '}';
            _CashFrmData = JSON.parse('['+obj+']');
            _Form_Serialize.SetFormData(JSON.parse(obj), _EditOrBrow);
        }
        catch (err) {
            _ErrMsg(err.message);
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
			var str1=Str.replace(/\*/g, "/");
			var str=str1.replace(/\-/g, "+");
			
            BaseStr=base.decode(str);            
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