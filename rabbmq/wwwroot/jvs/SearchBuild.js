var SelOrConst;
var BuildSearch = {
    Create: function (ListConFig) {
        var Query = ListConFig.split(',');  
        for (var x = 0; x < Query.length; x++) {
            var S = Query[x].split(':');
            if (S[3] == "1") {//允许查询
                var box = document.getElementById('FrmNode');
                var o = document.getElementsByTagName('label');
                var Node = o[o.length-1];
                var label = document.createElement("label");
                label.style.width = '400px';
                label.innerHTML = '<span> ' + S[2] + '&nbsp;</span>';
                switch (S[4]) {
                    case "I":
                    case "F":
                        label.innerHTML += '<input id="' + S[0] + '" type="text" name="'
                            + S[0] + '" placeholder="Please Input" />';
                        break;
                    case "D":
                        label.innerHTML += '<input id="' + S[0] + '" type="text" name="'
                            + S[0] + '" style="margin-top:5px;"' +
                            ' placeholder="Please Input" onfocus = "DatePick.Date(\'#' + S[0] + '\')" />';                        
                        break;
                    /*字符查询有常量和非常量*/
                    case "N":                
                        if (S[5] == '1') {//常量
                            label.innerHTML += '<select id="' + S[0] + '" name="' + S[0] + '"></select>';
                            SelOrConst = 0;//常量
                        }
                        else {
                            if (S[7] == '1') {
                                //sel查询                                       
                                label.innerHTML += '<select id="' + S[0] + '" name="' + S[0] + '"></select>';
                                SelOrConst = 1;
                            } else {//普通查询
                                label.innerHTML += '<input id="' + S[0] + '" type="text" name="'
                                    + S[0] + '" placeholder="Please Input" />';
                            }
                        }
                        break;
                }                                            
                box.insertAfter(label, Node);
            }
        }
        _Form_Serialize.SetFormAdd();
    },
    CreateMongo: function (OnwerFields) {
        var Query = OnwerFields.length ;
        for (var x = 0; x < Query; x++) {
            var json = OnwerFields[x];
            if (json.ifsearch == true) {
                var box = document.getElementById('FrmNode');
                var o = document.getElementsByTagName('label');
                var Node = o[o.length - 1];
                var label = document.createElement("label");
                label.style.width = '400px';
                label.innerHTML = '<span> ' + json.name + '&nbsp;</span>';
                switch (json.type) {
                    case "I":
                    case "F":
                        label.innerHTML += '<input id="' + json.name + '" type="text" name="'
                            + json.name + '" placeholder="Please Input" />';
                        break;
                    case "D":
                        label.innerHTML += '<input id="' + json.name + '" type="text" name="'
                            + json.name + '" style="margin-top:5px;"' +
                            ' placeholder="Please Input" onfocus = "DatePick.Date(\'#' + json.name + '\')" />';
                        break;
                    /*字符查询有常量和非常量*/
                    case "N":
                        label.innerHTML += '<input id="' + json.name + '" type="text" name="'
                            + json.name + '" placeholder="Please Input" />';
                        break;
                }
                box.insertAfter(label, Node);
            }
        }
    }
    ,
    PackParas: function (Form) {
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
                                        var opt = {
                                            FieldName: current.name,
                                            FieldType: Win.GetFieldType(current.name),
                                            FieldValue: _Str_Angement._StrRep(optionValue)
                                        };
                                        Win.SearchField.push(opt);
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
                                var opt = {
                                    FieldName: current.name,
                                    FieldType: Win.GetFieldType(current.name),
                                    FieldValue: rchk
                                };
                                Win.SearchField.push(opt);
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
                                var opt = {
                                    FieldName: current.name,
                                    FieldType: Win.GetFieldType(current.name),
                                    FieldValue: _Str_Angement._StrRep(current.value)
                                };
                                Win.SearchField.push(opt);
                            }
                        }
                }
            }
        }
    }
};
    