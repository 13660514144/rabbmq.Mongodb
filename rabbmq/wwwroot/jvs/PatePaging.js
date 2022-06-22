var PagingMode;
var PagingResult;
var _PageMothed;  //初始服务务参数寄 存
var _PageSeverCode;//初始服务务参数寄 存
var _SearchPara = {};//查询参数，同查询窗口传递回来，查询窗口只做查询参数封装
/*
 $.isEmptyObjec({}) Jquery 判断JSON对象是否空 返回0  OR 1  
 */
/*分页参数 */
function GetPagePara() {
    try {
        var Page = {
            StartPapge: (document.getElementById('CurrentPage').innerText == '' ? 0
                : parseInt(document.getElementById('CurrentPage').innerText)),
            Rpage: (document.getElementById('Rpage').innerText == '' ? 0
                : parseInt(document.getElementById('Rpage').innerText)),
            Rnum: (document.getElementById('Rnum').innerText == '' ? 0
                : parseInt(document.getElementById('Rnum').innerText)),
            MaxNum: (document.getElementById('MaxNum').innerText == '' ? 0
                : parseInt(document.getElementById('MaxNum').innerText)),
            MinNum: (document.getElementById('MinNum').innerText == '' ? 0
                : parseInt(document.getElementById('MinNum').innerText)),
            LastID: (document.getElementById('LastID').innerText == '' ? 0
                : parseInt(document.getElementById('LastID').innerText)),
            PagingMode: ''
        };
        return Page;
    }
    catch (err) {
        var pageno = {
            StartPapge: 0,
            Rpage: 0,
            Rnum: 0,
            MaxNum: 0,
            MinNum: 0,
            LastID: 0,
            PagingMode: 'None'
        };
        return pageno
    }
}

/*分页参数处理*/
function SetPagePara(pageobj) {
    try {
        document.getElementById('CurrentPage').innerText = pageobj.CurrentPage;
        document.getElementById('Rpage').innerText = pageobj.Rpage;
        document.getElementById('Rnum').innerText = pageobj.Rnum;

        document.getElementById('MaxNum').innerText = pageobj.MaxNum;
        document.getElementById('MinNum').innerText = pageobj.MinNum;
        document.getElementById('LastID').innerText = pageobj.LastID;
    }
    catch (err) {

    }
}
/*清理分页参数 */
function ClearPagePara() {
    try {
        document.getElementById('CurrentPage').innerText = '';
        document.getElementById('Rpage').innerText = '';
        document.getElementById('Rnum').innerText = '';

        document.getElementById('MaxNum').innerText = '';
        document.getElementById('MinNum').innerText = '';
        document.getElementById('LastID').innerText = '';
    }
    catch (err) {

    }
}
/*首面*/
function Spage_Click() {
    _RunListMothed("S");
}
function Npage_Click() {
    _RunListMothed("N");
}
function Ppage_Click() {
    _RunListMothed("P");
}
function Epage_Click() {
    _RunListMothed("E");
}
function _RunListMothed(QueryType) {
    PagingResult = GetPagePara();
    var FlgType = QueryType;
    var Run = false;
    switch (QueryType) {
        case 'S':
            Run = (PagingResult.StartPapge > 1) ? true : false;
            break;
        case 'N':
            Run = (PagingResult.StartPapge < PagingResult.Rpage) ? true : false;
            break;
        case 'P':
            Run = (PagingResult.StartPapge > 1) ? true : false;
            break;
        case 'E':
            Run = (PagingResult.StartPapge < PagingResult.Rpage) ? true : false;
            break;
        case 'C'://当前页参数
            //FlgType = 'N';
            //PagingResult.MinNum = (PagingResult.MaxNum == 0) ? PagingResult.MaxNum : PagingResult.MaxNum + 1;
            //PagingResult.StartPapge = PagingResult.StartPapge - 1;// (PagingResult.StartPapge == 1) ? PagingResult.StartPapge : PagingResult.StartPapge - 1;
            Run = true;
            break;
        case 'T':
            Run = true;
            ClearPagePara();
            PagingResult = GetPagePara();
            break;
        default:
            Run = true;
            break;
    }
    if (Run == true) {
        PagingResult.PagingMode = FlgType;
        var s = {
            ParentData: _SearchPara,
            PagePara: PagingResult
        };
        _ParaMethod = _PageMothed;
        _servercode = _PageSeverCode;
        var data = _PackClient(s, [], [], []);
        _AsyncRequest._AsyncPost(data, '_CallBackPaging', '/ident.ashx');
    }
    else {       
        _Form_Serialize.BtnObjStatus(_CurrentBtn, false);
        _closeiframe();
    }
}
function _CallBackPaging(data) {    
    try {
        _ErrMsg(data.Msg);                
        if (data.scuess) {
            _PagePath = data.Result.PagePath;
            _tb_elements._ListBrowTb('SubList', data.Result.ListConFig, data.Result.Data, true, 'HEAD', 'R');
            if (data.IsResult) {
                SetPagePara(data.Result.PagePara);
                _CashListData = data.Result.Data;//缓存结果集            
                _tb_elements._ListBrowTb('SubList', data.Result.ListConFig, data.Result.Data, true, 'BODY', 'R');                
            }           
        }        
    }
    catch (err) {
        _ErrMsg(err.message);        
    }    
}