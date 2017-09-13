var xmlHttp=null, map=null, timer=null, menutimer=null;
var mode="", id=0, from=0;

function str_replace(search, replace, txt) {
while(txt.indexOf(search) > -1) {
	txt = txt.replace(search, replace); }
return txt; }

function nl2br(txt) {
return str_replace("\n", "<br>", txt); }

function GetObj(objname) {
if(objname=="") return false;
var found = document.getElementById(objname);
if(!found) { if(document.all) {	found = document.all(objname); }}
if(!found) return false;
return found;
}

function bindElement(el, event, func) {
    if (el.addEventListener)
        el.addEventListener(event, func, false);
    else if (window.attachEvent)
        el.attachEvent("on" + event, func);
    else
        console.log('error binding event');
}
function dragOBJ(d,e) {

    function drag(e) { if(!stop) { d.style.top=(tX=xy(e,1)+oY-eY+'px'); d.style.left=(tY=xy(e)+oX-eX+'px'); } }

    var oX=parseInt(d.style.left),oY=parseInt(d.style.top),eX=xy(e),eY=xy(e,1),tX,tY,stop;

    document.onmousemove=drag; document.onmouseup=function(){ stop=1; document.onmousemove=''; document.onmouseup=''; };

}
function GetXmlHttpObject() {
var xmlHttp;
try { xmlHttp = new ActiveXObject('Msxml2.XMLHTTP'); }
catch (e) {
	try { xmlHttp = new ActiveXObject('Microsoft.XMLHTTP'); }
	catch (E) { xmlHttp = false; }}
if(!xmlHttp && typeof XMLHttpRequest != 'undefined') { xmlHttp = new XMLHttpRequest(); }
return xmlHttp; }

function AJAX(url, params, OnChange) {
    xmlHttp = GetXmlHttpObject(); if (xmlHttp == null) return;
    bindElement(xmlHttp, 'loadend', GetTnv);
if(OnChange!= null) xmlHttp.onreadystatechange = OnChange;
xmlHttp.open('GET', url, true);
xmlHttp.setRequestHeader('Content-Type','application/x-www-form-urlencoded');
xmlHttp.send(params);
}
function AJAX2(url, params, OnChange) {
    xmlHttp = GetXmlHttpObject(); if (xmlHttp == null) return;
    if (OnChange != null) xmlHttp.onreadystatechange = OnChange;
    xmlHttp.open('GET', url, true);
    xmlHttp.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
    xmlHttp.send(params);
}
function testAjax(url) {
  $.ajax({
    url: url,
	type: "GET",
    success: function(data) {
	//alert(data);
      return data; 
    }
  });
}

function AJAXStatus(txt) {
div = GetObj('loading-div');
if(!div) return;
if(div.style.display == 'none') { ShowStatusDiv(); }
divtxt = GetObj('loading-text');
if(!divtxt) return;
if(txt) { if(txt!='') { divtxt.innerHTML = txt; }}
return true; }

function AJAXResult() {
if (xmlHttp.readyState==4 || xmlHttp.readyState=="complete") {
	HideStatusDiv();
	alert(xmlHttp.responseText);
	return; }
if(lang=="EN") AJAXStatus("Loading data...");
else AJAXStatus("Загрузка данных..."); }

function GetClientWidth() {
return document.compatMode=='CSS1Compat' && !window.opera?document.documentElement.clientWidth:document.body.clientWidth; }

function GetClientHeight() {
return document.compatMode=='CSS1Compat' && !window.opera?document.documentElement.clientHeight:document.body.clientHeight; }

function GetBodyScrollLeft() {
return self.pageXOffset || (document.documentElement && document.documentElement.scrollLeft) || (document.body && document.body.scrollLeft); }

function GetBodyScrollTop() {
return self.pageYOffset || (document.documentElement && document.documentElement.scrollTop) || (document.body && document.body.scrollTop); }

function GetClientCenterX() {
return parseInt(GetClientWidth()/2)+GetBodyScrollLeft(); }

function GetClientCenterY() {
return parseInt(GetClientHeight()/2); }

function ShowStatusDiv() {
div = GetObj("loading-div");
if(!div) return false;
div.style.display = "block";
div.style.position = "absolute";
div.style.zIndex = 999;
div.style.background = "#EAEBF0";
div.style.margin = "0px";
div.style.padding = "5px";
div.style.border = "1px solid";
div.style.fontSize = "12px";
div.style.fontFamily = "Verdana, Tahoma, Arial, sans-serif";
var dwidth  = parseInt(div.style.width)?parseInt(div.style.width):parseInt(div.offsetWidth);
var dheight = parseInt(div.style.height)?parseInt(div.style.height):parseInt(div.offsetHeight);
div.style.left = (GetClientCenterX()-dwidth/2)+"px";
//div.style.top = (GetClientCenterY()-dheight/2)+"px";
return true; }

function HideStatusDiv() {
div = GetObj("loading-div");
if(!div) return false;
div.style.display="none";
div.style.zIndex=-1;
return true; }

function trim(val) {
S = new String(val);
return S.replace(/(^\s+)|(\s+$)/g, ""); }

function OpenURL(url) {
url = trim(url); if(url=="") return;
document.location.assign(url); }

function NN(value) {
if(value>=10) return value; 
return "0" + value; }

function JSDate(date) {
return NN(date.getDate()) + "." + NN(date.getMonth()+1) + "." + date.getFullYear(); }

function JSTime(date, hm) {
if(hm) return NN(date.getHours()) + ":" + NN(date.getMinutes());
return NN(date.getHours()) + ":" + NN(date.getMinutes()) + ":" + NN(date.getSeconds()); }

function GetWeekDay(dt) {
return ["Воскресенье","Понедельник","Вторник","Среда","Четверг","Пятница","Суббота"][dt.getDay()]; }

function ShowUTime(time, hm) {
var dtime = new Date(time*1e3);
var date = JSDate(dtime);
var today = new Date();
if(date == JSDate(today)) return "Сегодня в " + JSTime(dtime,hm);
var yesday = new Date(today.getTime()-864*1e5);
if(date == JSDate(yesday)) return "Вчера в " + JSTime(dtime,hm);
return date + " в " + JSTime(dtime,hm); }

function ShowSubMenu(obj, divname) {
clearTimeout(menutimer);
HideSubMenu("stypes");
div = GetObj(divname);
div.style.zIndex = 1000;
div.style.display = "block";
div.style.top = (getPos(obj,"Top") + obj.offsetHeight + 5) + "px";
div.style.left = getPos(obj,"Left") + "px"; }

function MenuOut(divname) {
menutimer = setTimeout("HideSubMenu('"+divname+"')",1000); }

function getPos(obj, sProp) {
var iPos=0;
while(obj!=null) {
	iPos += obj["offset" + sProp];
	obj = obj.offsetParent; }
return iPos; }

function HideSubMenu(divname) {
div = GetObj(divname);
if(!div) return;
div.style.display = "none";
div.style.zIndex = -1;
return; }


function ShowFormDiv(divname) {
var div = GetObj("shadow");
if(!div) return;
div.style.display = "block";
div.style.zIndex = 5;
div.style.opacity = 0.4;
div.style.filter = 'alpha(opacity=40)';
div = GetObj(divname);
if(!div) return;
div.style.top = "40px";
div.style.left = "0px";
div.style.display = "block";
div.style.position = "absolute";
div.style.zIndex = 99;
div.style.background = "#EAEBF0";
div.style.margin = "0px";
div.style.padding = "5px";
div.style.border = "1px solid";
var dwidth = parseInt(div.style.width)?parseInt(div.style.width):parseInt(div.offsetWidth);
var dheight = parseInt(div.style.height)?parseInt(div.style.height):parseInt(div.offsetHeight);
if(dwidth >= GetClientWidth()) div.style.left = "0px";
	else div.style.left = (GetClientCenterX()-dwidth/2)+"px";
var dtop = GetClientCenterY()-dheight/2;
if(dtop<40) dtop = 40;
div.style.top = dtop+"px";
}

function LoadDialog(id, idDialog) {
var url = "";
if (id != undefined && idDialog == 1) {
	$('#StartTask').prop('disabled', false);
	PM = GetPM(id);
	var address = PM.properties.get('address');
	document.all.address.value = address;
	document.getElementById('idMD_D').value = PM.properties.get('idMD');
}

//if (from && (from>0)) url += "&from=" + from;
AJAX(url, "", ShowDialog);
//ShowDialog();
}

function ShowDialog() {
//if (xmlHttp.readyState==4 || xmlHttp.readyState=="complete") {
        HideStatusDiv();
        
	HideDialog("banner");
        ShowFormDiv("dialog");
	return;
	//}
if(lang=="EN") AJAXStatus("Loading dialog...");
else AJAXStatus("Загрузка диалога..."); }
function ShowGraph(id) {
        HideStatusDiv();
	HideDialog("banner");
        ShowFormDiv("graph_peretop");
	ShowGraphPeretop(id);
	return;
	//}
if(lang=="EN") AJAXStatus("Loading dialog...");
else AJAXStatus("Загрузка диалога..."); }
function ShowDialogTicket(id) {
        HideStatusDiv();
	    HideDialog("banner");
        //ShowFormDiv("dialog_ticket");
		ShowWindowTickets(pmid);
	//refreshGrid_tickets(this.checked);
	return;
	//}
if(lang=="EN") AJAXStatus("Loading dialog...");
else AJAXStatus("Загрузка диалога..."); }
function ShowDialogUkuts() {
	HideDialog("banner");
	refreshGrid_ukuts();
        ShowFormDiv("dialog_ukuts");
    }
function ShowDialogValues() {
	HideDialog("banner");
	refreshGrid_values();
    ShowFormDiv("dialog_values");
}

	function ShowDialogRegulator() {
			 HideDialog("banner");
			 var PM = GetPM(pmid);
			 var con = PM.properties.get('balloonContentBody');
			 var mySVG = document.getElementById("regulator_svg");
			 var svgDoc;
			 var tustav;
			 var btnup;
			 var btndown;
	mySVG.addEventListener("load",function() {
	svgDoc = mySVG.contentDocument;
	tustav = svgDoc.getElementById("svg_156");
	toper = svgDoc.getElementById("svg_155");
	btnup = svgDoc.getElementById("btnup");
	btndown = svgDoc.getElementById("btndown");
		//уставка
			var result = con.replace(/<[^>]+>/g,'');
			var ustavka = result.indexOf("Уставка tГвс");
			var value_ustav = result.substr(ustavka+13, 5);
			//Температура на регуляторе
			var t_regulat = result.indexOf("Температура на регуляторе");
			var value_t_regulat = result.substr(t_regulat+26, 5);
			//Обновляем SVG
			tustav.textContent=value_ustav;
			toper.textContent=value_t_regulat;
			//закончили вывод
	addEvent(btnup, "click", function () {
		alert("+1");
			});
		addEvent(btndown, "click", function () {
		alert("-1");
			});
					}, false);
 
    ShowFormDiv("dialog_regulator");
    }
	function addEvent(obj, evt, func) {
    if (obj.addEventListener) {
        obj.addEventListener(evt, func, false)
    } else if (obj.attachEvent) {
        obj.attachEvent("on" + evt, func);
    } else {
        alert("no success adding event");
    }
}
function HideDialog(divname) {
if(!divname) divname="dialog";
var div = GetObj(divname);
if(!div) return;
div.style.display = "none";
div = GetObj("shadow");
div.style.display = "none"; }


function sleep(milliseconds) {
  var start = new Date().getTime();
  for (var i = 0; i < 1e7; i++) {
    if ((new Date().getTime() - start) > milliseconds){
      break;
    }
  }
}
function showForm(winId, node)
{
            $$(winId).getBody().clear();
            $$(winId).show(node);
            $$(winId).getBody().focus();
}
function ShowWindowUkuts()
{
	webix.ui.datafilter.countRows = webix.extend({
	refresh:function(master, node, value){
    node.firstChild.innerHTML = master.count();
  }
}, webix.ui.datafilter.summColumn);
	
webix.ui({
    view: "window", id: "utickets", move: true, resize: true,
    head:{
		view:"toolbar", cols:[
						{view:"label", label: "Таблица объектов диспетчерской связи" },
						
						{ view:"button", label: 'Excel', width:100, align: 'right', click:function(){
							webix.toExcel($$("grid_ukuts"));
							}},
							{ view:"button", label: 'Закрыть', width:100, align: 'right', click:function(){this.getTopParentView().close();}},
						]
	},
	body:{
		 rows:[
                {
                 view: "richselect", width: 300,
                  labelAlign: "right",
                 value: 1, options: [
                     { id: 1, value: "Общие" },
                     { id: 2, value: "Связь" }
                 ],
                 on: {
                     "onChange": function (newv, oldv) {
                         $$("grid_ukuts").showColumnBatch(newv);
                     },
                 }
             
                },
                {
					view:"treetable",
                    id: "grid_ukuts", height: 500, select: "row", visibleBatch: 1,

				columns:[
					{ id:"idUkut",	header:"#", width:50},
					{ id:"address",	header:["Адрес",{content:"textFilter"}], width:200, sort:"string", footer:{text:"Всего:", colspan:3} },
					{ id:"geu",	header:["ЖЭУ",{content:"selectFilter"}],    width:50, sort:"int", css:{'text-align':'center'}},
					{ id:"idMD",	header:"idMD", hidden:true},
					{ id:"devName",	header:["Прибор",{content:"textFilter"}],   width:160, sort:"string" },
					{ id:"param_value",	header:["ГВС",{content:"textFilter"}],    width:60},
					{ id:"type_text",	header:["Состояние",{content:"selectFilter"}],    width:90, sort:"string" },
					{ id:"date",	header:"Обновлено", 
							template:function(obj){
							//var date = new Date(parseInt(obj.date.replace(/\/Date\((\d+)\)\//, '$1')));
							var date = moment(obj.date).fromNow();
							return date;
						},
					width:160, sort:"date", format:webix.i18n.dateFormatStr},
					/*{ id:"zabbix_time",	header:"На связи с ",
							template:function(obj){
							var date = new Date(parseInt(obj.zabbix_time.replace(/\/Date\((\d+)\)\//, '$1')));
							date = moment(date).fromNow();
							return date;
						},
					width:190, sort:"date", format:webix.i18n.dateFormatStr, hidden:true},
                    */
					{ id:"countTrub",	header:["Труб",{content:"selectFilter"}],    width:50, sort:"int", css:{'text-align':'center'}},
                    { id: "commlinestatus", header: ["Статус", { content: "selectFilter" }], width: 200, batch: 2},
                    { id: "teplo_source", header: ["Источник", { content: "selectFilter" }], width: 200, sort: "string", footer: { content: "countRows" }, batch: 1 },
                    {
                        template: "<input type='button' class='webix_button webixtype_danger' value='Архив'></span>"
                    },
                    {
                        id: "last_day_arc", header: "Последний суточный архив", width: 160, batch: 2, sort: "date", format: webix.i18n.dateFormatStr, template: function (obj) {
                            var last_day_arc = moment(obj.last_day_arc).fromNow();
                            return last_day_arc;
                        },
                  
                    },
                    {
                        id: "last_hour_arc", header: "Последний часовой архив", width: 160, batch: 2, sort: "date", batch: 2,format: webix.i18n.dateFormatStr, template: function (obj) {
                            var last_hour_arc = moment(obj.last_hour_arc).fromNow();
                            return last_hour_arc;
                        },

                    },
                       {
                           template: "<input type='button' class='webix_button webixtype_form' value='Опрос'></span>", batch: 2,
                       },
					
				],
				onClick: {
				    "webixtype_danger": function (ev, id) {
				        var record = this.getItem(id);
				        dateFrom = moment().startOf('month').format("YYYY.MM.DD HH:mm");
				        dateTo = moment().format("YYYY.MM.DD HH:mm");
				        ShowWindowArchives(record.idMD, record.address, dateFrom, dateTo);
				    },
				    "webixtype_form": function (ev, id) {
				        var record = this.getItem(id);
				        var params = '?KPNum=' + record.idMD;
				        AJAX('ukut.asmx/StartProccessArc' + params, params, function () {
				            if (xmlHttp.readyState == 4 || xmlHttp.readyState == "complete")
				            {

				            }
				        });
				    },
				},
				 sort:{
					by:"#address#",
					dir:"asc",
					as:"string"
						},
                    on:{
						"onItemDblClick":webix.once(function(id, e, node){
							 var item = this.getItem(id);
							ZoomIn(item.idUkut);
							this.getTopParentView().hide();
						}),
						"onLoadError":webix.once(function(){ 
							this.hide();
						}),
						"onBeforeLoad":function(){
							 this.showOverlay("Загрузка...");
						},
						"onAfterLoad":function(){
							 this.hideOverlay();
							},
		
					},
                	scheme:{
                    $change:function(item){
						if (item.idUkut === 'Завышение'){
							item.$css = "highlight";
						}
						if (item.idUkut === 'Нет связи'){
							item.$css = "errlight";
						}
						if (item.idUkut === 'Занижение'){
							item.$css = "lowlight";
						}
					}
				},
                rowLineHeight:15, rowHeight:35,
				autowidth:true,
				footer:true
				//autoheight:true
                }
            ],
	
	}
}).show();
			webix.extend($$("grid_ukuts"), webix.ProgressBar);
			$$("grid_ukuts").load("ukut.asmx/UkutsMaps?type=0,1,2,3,4,10&temp_mode=1&peret_mode=1&geu=1,2,3,4,5,6,7,8,9,10&res_mode=table");
			$$("grid_ukuts").sort("#address#","asc","string");
}
function round(a,b) {
 b=b || 0;
 return Math.round(a*Math.pow(10,b))/Math.pow(10,b);
}
function GetPMonArchive(id)
{
    PM = GetPM(id);
    var address = PM.properties.get('address');
    idMD = PM.properties.get('idMD');
    dateFrom = moment().startOf('month').format("YYYY.MM.DD HH:mm");
    dateTo = moment().format("YYYY.MM.DD HH:mm");
    ShowWindowArchives(idMD, address, dateFrom, dateTo)
}
function PrepareArchive()
{
    var idMD = $$("idMD").getValue();
    var arc = $$("arc").getValue();
    if (arc == 2) {
        arc = "days";
    }
    else {
        arc = "hour"
    }
    var dateFrom = $$("dateFrom").getValue();
    var dateTo = $$("dateTo").getValue();
    var url = 'ukut.asmx/GetArchive?arc=' + arc + '&idMD=' + idMD + '&dateFirst="' + dateFrom + '"&dateLast="' + dateTo + '"';
    $$("garchives").clearAll();
    $$("garchives").load(url);
}
/*
function ShowWindowArchives(idMD, address, dateFrom, dateTo)
{
	var mm = moment().startOf('month');
	
	webix.ui.datafilter.countRows = webix.extend({
	refresh:function(master, node, value){
    node.firstChild.innerHTML = master.count();
  }
}, webix.ui.datafilter.summColumn);
webix.i18n.setLocale("ru-RU");	
webix.ui({
	view:"window",  id:"warchives",left:50, top:50,	move:true, resize: true,
    head:{
		view:"toolbar", cols:[
						{view:"label", label: "Таблица архивов по адресу: "+address },
				
						{ view:"button", label: 'Excel', width:100, align: 'right', click:function(){
							webix.toExcel($$("garchives"));
							}},
									{ view:"button", label: 'Закрыть', width:100, align: 'right', click:function(){this.getTopParentView().close();}},
							
						]
	},
	body:{
		 rows:[
					{ cols:[ //2nd row
						{
						    rows: [
                            { view: "label", id: "idMD", label: idMD, value: idMD, hidden: true },
							{ view:"radio", customRadio:false, name:"arc", id:"arc", label:"Архив: ", value:2, options:[{ id:1, value:"Часовые" }, { id:2, value:"Суточные" }] },
							{ view: "datepicker", id: "dateFrom", timepicker: true, width: 300, label: "От", value: dateFrom, name: "start", stringResult: true, format: "%d  %M %Y %H:%i" },
							{ view: "datepicker", id: "dateTo", suggest: { type: "calendar", body: { maxDate: new Date() } }, value: dateTo, timepicker: true, width: 300, label: "До", name: "end", stringResult: true, format: "%d %M %Y %H:%i" },
							]},
							{ view:"button", value:"Получить", width: 150, click:"PrepareArchive"},
							{ view:"button", id:"startArc", value:"Опрос", width: 150, click:"StartProccessArc"},
						]},
					
						

                {
					view:"datatable",
					id: "garchives", width: 800, height: 400, select: "row", scrollX: true,

				columns:[
					{ id:"Date",	header:"Дата", fillspace:true,
							template:function(obj){
							var date = moment(obj.Date);
							return date.format("DD.MM.YYYY HH:mm"); 
						},
					width:160, sort:"date", format:webix.i18n.dateFormatStr},
					//TB1
					{ id:"TB1_M1",	header:["M1, т"],template:function(obj){
							return round(obj.TB1_M1,2);
						},  width:110,fillspace:true},		
					{ id:"TB1_M2",	header:["M2, т"],template:function(obj){
							return round(obj.TB1_M2,2);
						},  width:110, fillspace:true},	
					{ id:"TB1_M3",	header:["M3, т"],template:function(obj){
							return round(obj.TB1_M3,2);
						},  width:110, fillspace:true, hidden:true},
					{ id:"TB1_t1",	header:["t1, C"],template:function(obj){
							return round(obj.TB1_t1,2);
						},  width:110, fillspace:true},
					{ id:"TB1_t2",	header:["t2, C"], template:function(obj){
							return round(obj.TB1_t2,2);
						}, width:110, fillspace:true},
					{ id:"TB1_t3",	header:["t3, C"], template:function(obj){
							return round(obj.TB1_t3, 2);
						}, width:110,fillspace:true, hidden:true},
					{ id:"TB1_tx",	header:["tx, C"],template:function(obj){
							return round(obj.TB1_tx,2);
						},  width:110,fillspace:true, hidden:true},
					{ id:"TB1_tv",	header:["tv, C"], template:function(obj){
							return round(obj.TB1_tv,2);
						}, width:110,fillspace:true, hidden:true},					
					{ id:"TB1_V1",	header:["V1, м3"],template:function(obj){
							return round(obj.TB1_V1,2);
						},  width:110,fillspace:true, hidden:true},
					{ id:"TB1_V2",fillspace:true,	header:["V2, м3"],template:function(obj){
							return round(obj.TB1_V2,2);
						},  width:110,fillspace:true, hidden:true},
					{ id:"TB1_V3",	header:["V3, м3"],template:function(obj){
							return round(obj.TB1_V3,2);
						},  width:110,fillspace:true, hidden:true},	
					{ id:"TB1_P1",	header:["P1, ат"], 	template:function(obj){
							return round(obj.TB1_P1,2);
						}, width:110, fillspace:true},
					{ id:"TB1_P2",	header:["P2, ат"], template:function(obj){
							return round(obj.TB1_P2,2);
						},  width:110, fillspace:true},						
					{ id:"TB1_Q", header:["Q, Гкал"],template:function(obj){
							return round(obj.TB1_Q,2);
						},  width:110, hidden:true, fillspace:true},	
					{ id:"TB1_Ti", header:["Ti"],template:function(obj){
							return round(obj.TB1_Ti,2);
						},  width:110, hidden:true, fillspace:true},	
					//TB2
					{ id:"TB2_M1",	header:["M1, т"],template:function(obj){
							return round(obj.TB2_M1,2);
						},  width:110, fillspace:true},		
					{ id:"TB2_M2",	header:["M2, т"],template:function(obj){
							return round(obj.TB2_M2,2);
						},  width:110, fillspace:true},	
					{ id:"TB2_M3",  header:["M3, т"],template:function(obj){
							return round(obj.TB2_M3,2);
						},  width:110, hidden:true, fillspace:true},
					{ id:"TB2_t1",	header:["t1, C"],template:function(obj){
							return round(obj.TB2_t1,2);
						},  width:110,fillspace:true},
					{ id:"TB2_t2",	header:["t2, C"], template:function(obj){
							return round(obj.TB2_t2,2);
						}, width:110, fillspace:true},
					{ id:"TB2_t3", header:["t3, C"], template:function(obj){
							return round(obj.TB2_t3, 2);
						}, width:110, hidden:true, fillspace:true},
					{ id:"TB2_t3",	header:["tx, C"],template:function(obj){
							return round(obj.TB2_tx,2);
						},  width:110, hidden:true, fillspace:true},
					{ id:"TB2_tv",	header:["tv, C"], template:function(obj){
							return round(obj.TB2_tv,2);
						}, width:110, hidden:true, fillspace:true},					
					{ id:"TB2_V1",	header:["V1, м3"],template:function(obj){
							return round(obj.TB2_V1,2);
						},  width:110, hidden:true, fillspace:true},
					{ id:"TB2_V2",	header:["V2, м3"],template:function(obj){
							return round(obj.TB2_V2,2);
						},  width:110, hidden:true, fillspace:true},
					{ id:"TB2_V3",	header:["V3, м3"],template:function(obj){
							return round(obj.TB2_V3,2);
						},  width:110, hidden:true, fillspace:true},	
					{ id:"TB2_P1",	header:["P1, ат"], 	template:function(obj){
							return round(obj.TB2_P1,2);
						}, width:110, fillspace:true},
					{ id:"TB2_P2",	header:["P2, ат"], template:function(obj){
							return round(obj.TB2_P2,2);
						},  width:110, fillspace:true},							
					{ id:"TB2_Ti", header:["Ti"],template:function(obj){
							return round(obj.TB2_Ti,2);
						},  width:110,	hidden:true, fillspace:true},	
					{ id:"TB2_Q", header:["TB2 Qгвс"],  width:110, fillspace:true, hidden:true},						
				],
                    on:{
						"onItemDblClick":webix.once(function(id, e, node){
							 var item = this.getItem(id);
							ZoomIn(item.idUkut);
							this.getTopParentView().hide();
						}),
						"onLoadError":webix.once(function(){ 
							this.hide();
						}),
						"onBeforeLoad":function(){
							 this.showOverlay("Загрузка...");
						},
						"onAfterLoad":function(){
							 this.hideOverlay();
							 if (!this.count())
							this.showOverlay("Нет данных за указанный период");
							},
		
					},
                
				autowidth:true,
				//headermenu:true

				
                }
            ],
	
	}
}).show();
webix.extend($$("garchives"), webix.ProgressBar);
PrepareArchive();
webix.i18n.setLocale("ru-RU");
}
*/
function ShowWindowArchives(idMD, address, dateFrom, dateTo) {
    var mm = moment().startOf('month');
    webix.ui.datafilter.countRows = webix.extend({
        refresh: function (master, node, value) {
            node.firstChild.innerHTML = master.count();
        }
    }, webix.ui.datafilter.summColumn);
    webix.i18n.setLocale("ru-RU");
    webix.ui({
        view: "window", id: "warchives", top: 40, left: 60, borderless: true,
        head: {
            view: "toolbar", cols: [
                            { view: "label", label: "Таблица архивов по адресу: " + address },

                            {
                                view: "button", label: 'Excel', width: 100, click: function () {
                                    webix.toExcel($$("garchives"), {
                                        filename: address,
                                        name: address,
                                        columns: {
                                            "Date": { header: "Date", width: 125 },
                                            "TB1_M1": { header: "TB1_M1", width: 50 },
                                            "TB1_M2": { header: "TB1_M2", width: 50 },
                                            "TB1_M3": { header: "TB1_M3", width: 50 },
                                            "TB1_t1": { header: "TB1_t1", width: 50 },
                                            "TB1_t2": { header: "TB1_t2", width: 50 },
                                            "TB1_t3": { header: "TB1_t3", width: 50 },
                                            "TB1_tx": { header: "TB1_tx", width: 50 },
                                            "TB1_tv": { header: "TB1_tv", width: 50 },
                                            "TB1_V1": { header: "TB1_V1", width: 50 },
                                            "TB1_V2": { header: "TB1_V2", width: 50 },
                                            "TB1_V3": { header: "TB1_V3", width: 50 },
                                            "TB1_P1": { header: "TB1_P1", width: 50 },
                                            "TB1_P2": { header: "TB1_P2", width: 50 },
                                            "TB1_Q": { header: "TB1_Q", width: 50 },
                                            "TB1_Ti": { header: "TB1_Ti", width: 50 },
                                            "TB1_Qg": { header: "TB1_Qg", width: 50 },
                                            "TB2_M1": { header: "TB2_M1", width: 50 },
                                            "TB2_M2": { header: "TB2_M2", width: 50 },
                                            "TB2_M3": { header: "TB2_M3", width: 50 },
                                            "TB2_t1": { header: "TB2_t1", width: 50 },
                                            "TB2_t2": { header: "TB2_t2", width: 50 },
                                            "TB2_t3": { header: "TB2_t3", width: 50 },
                                            "TB2_tx": { header: "TB2_tx", width: 50 },
                                            "TB2_tv": { header: "TB2_tv", width: 50 },
                                            "TB2_V1": { header: "TB2_V1", width: 50 },
                                            "TB2_V2": { header: "TB2_V2", width: 50 },
                                            "TB2_V3": { header: "TB2_V3", width: 50 },
                                            "TB2_P1": { header: "TB2_P1", width: 50 },
                                            "TB2_P2": { header: "TB2_P2", width: 50 },
                                            "TB2_Q": { header: "TB2_Q", width: 50 },
                                            "TB2_Ti": { header: "TB2_Ti", width: 50 },
                                            "TB2_Qg": { header: "TB2_Qg", width: 50 },
                                        }

                                    });
                                }
                            },
                            { view: "button", label: 'Закрыть', width: 100, click: function () { this.getTopParentView().close(); } },

            ]
        },
        body: {
            rows: [
                       {
                           cols: [ //2nd row
                             {
                                 rows: [
                                        { view: "label", id: "idMD", label: idMD, value: idMD, hidden: true },
                                 {
                                     view: "radio", customRadio: false, name: "arc", id: "arc", label: "Архив: ", value: 2, options: [{ id: 1, value: "Часовые" }, { id: 2, value: "Суточные" }],
                                     on: {

                                         "onChange": function (newv, oldv) {
                                             PrepareArchive();
                                         },
                                     },
                                 },
                                 { view: "datepicker", id: "dateFrom", timepicker: true, width: 300, label: "От", value: dateFrom, name: "start", stringResult: true, format: "%d  %M %Y %H:%i" },
                                 { view: "datepicker", id: "dateTo", suggest: { type: "calendar", body: { maxDate: new Date() } }, value: dateTo, timepicker: true, width: 300, label: "До", name: "end", stringResult: true, format: "%d %M %Y %H:%i" },
                                 ]
                             },
                           {
                               view: "radio",
                               label: 'TB',
                               value: "TB1",
                               width: 150,
                               options: [{ value: "TB1" }, { value: "TB2" }],
                               on: {

                                   "onChange": function (newv, oldv) {

                                       if (newv === "TB2" && oldv === "TB1") {
                                           $$("garchives").showColumnBatch(2);
                                           $$("garchives").refresh();
                                           PrepareArchive();
                                       }
                                       if (newv === "TB1" && oldv === "TB2") {
                                           $$("garchives").showColumnBatch(1);
                                           $$("garchives").refresh();
                                           PrepareArchive();

                                       }
                                   },
                               },
                           },


                                 { view: "button", value: "Получить", width: 150, click: "PrepareArchive" },
                                 { view: "button", id: "startArc", value: "Опрос", width: 150, click: "StartProccessArc" },
                           ]
                       },



                   {
                       view: "datatable",
                       id: "garchives", borderless: true, visibleBatch: 1, height: 500,
                       css: "my_style",


                       columns: [
                           {
                               id: "Date", header: "Дата", adjust: "data",
                               template: function (obj) {
                                   var date = moment(obj.Date);
                                   return date.format("DD.MM.YYYY HH:mm");
                               },
                               sort: "date", format: webix.i18n.dateFormatStr
                           },
                           //TB1
                           {
                                          id: "TB1_SP", header: ["TB1_СП"], adjust: "all", batch: 1
                           },
                           {
                               id: "TB1_M1", header: ["TB1_M1"], template: function (obj) {
                                   return round(obj.TB1_M1, 2);
                               }, adjust: "all", batch: 1,
                           },
                           {
                               id: "TB1_M2", header: ["TB1_M2"], template: function (obj) {
                                   return round(obj.TB1_M2, 2);
                               }, adjust: "all", batch: 1,
                           },
                           {
                               id: "TB1_M3", header: ["TB1_M3"], template: function (obj) {
                                   return round(obj.TB1_M3, 2);
                               }, adjust: "all", batch: 1,
                           },
                           {
                               id: "TB1_t1", header: ["TB1_t1"], template: function (obj) {
                                   return round(obj.TB1_t1, 2);
                               }, adjust: "all", batch: 1,
                           },
                           {
                               id: "TB1_t2", header: ["TB1_t2"], template: function (obj) {
                                   return round(obj.TB1_t2, 2);
                               }, adjust: "all", batch: 1,
                           },
                           {
                               id: "TB1_t3", header: ["TB1_t3"], template: function (obj) {
                                   return round(obj.TB1_t3, 2);
                               }, adjust: "all", batch: 1,
                           },
                           {
                               id: "TB1_tx", header: ["TB1_tx"], template: function (obj) {
                                   return round(obj.TB1_tx, 2);
                               }, adjust: "all", batch: 1,
                           },
                           {
                               id: "TB1_tv", header: ["TB1_tv"], template: function (obj) {
                                   return round(obj.TB1_tv, 2);
                               }, adjust: "all", batch: 1,
                           },
                           {
                               id: "TB1_V1", header: ["TB1_V1"], template: function (obj) {
                                   return round(obj.TB1_V1, 2);
                               }, adjust: "all", batch: 1,
                           },
                           {
                               id: "TB1_V2", header: ["TB1_V2"], template: function (obj) {
                                   return round(obj.TB1_V2, 2);
                               }, adjust: "all", batch: 1,
                           },
                           {
                               id: "TB1_V3", header: ["TB1_V3"], template: function (obj) {
                                   return round(obj.TB1_V3, 2);
                               }, adjust: "all", batch: 1,
                           },
                           {
                               id: "TB1_P1", header: ["TB1_P1"], template: function (obj) {
                                   return round(obj.TB1_P1, 2);
                               }, adjust: "all", batch: 1,
                           },
                           {
                               id: "TB1_P2", header: ["TB1_P2"], template: function (obj) {
                                   return round(obj.TB1_P2, 2);
                               }, adjust: "all", batch: 1,
                           },
                           {
                               id: "TB1_Q", header: ["TB1_Q"], template: function (obj) {
                                   return round(obj.TB1_Q, 2);
                               }, adjust: "all", batch: 1,
                           },
                           {
                               id: "TB1_Ti", header: ["TB1_Ti"], template: function (obj) {
                                   return round(obj.TB1_Ti, 2);
                               }, adjust: "all", batch: 1,
                           },
                           {
                               id: "TB1_Qg", header: ["TB1_Qg"], template: function (obj) {
                                   return round(obj.TB1_Qg, 2);
                               }, adjust: "all", batch: 1,
                           },
                           //TB2
                           {
                               id: "TB2_SP", header: ["TB2_СП"], adjust: "all", batch: 2, hidden: true,
                           },
                           {
                               id: "TB2_M1", header: ["TB2_M1"], template: function (obj) {
                                   return round(obj.TB2_M1, 2);
                               }, adjust: "all", batch: 2, hidden: true,
                           },
                           {
                               id: "TB2_M2", header: ["TB2_M2"], template: function (obj) {
                                   return round(obj.TB2_M2, 2);
                               }, adjust: "all", batch: 2, hidden: true,
                           },
                           {
                               id: "TB2_M3", header: ["TB2_M3"], template: function (obj) {
                                   return round(obj.TB2_M3, 2);
                               }, adjust: "all", batch: 2, hidden: true,
                           },
                           {
                               id: "TB2_t1", header: ["TB2_t1"], template: function (obj) {
                                   return round(obj.TB2_t1, 2);
                               }, adjust: "all", batch: 2, hidden: true,
                           },
                           {
                               id: "TB2_t2", header: ["TB2_t2"], template: function (obj) {
                                   return round(obj.TB2_t2, 2);
                               }, adjust: "all", batch: 2, hidden: true,
                           },
                           {
                               id: "TB2_t3", header: ["TB2_t3"], template: function (obj) {
                                   return round(obj.TB2_t3, 2);
                               }, adjust: "all", batch: 2, hidden: true,
                           },
                           {
                               id: "TB2_tx", header: ["TB2_tx"], template: function (obj) {
                                   return round(obj.TB2_tx, 2);
                               }, adjust: "all", batch: 2, hidden: true,
                           },
                           {
                               id: "TB2_tv", header: ["TB2_tv"], template: function (obj) {
                                   return round(obj.TB2_tv, 2);
                               }, adjust: "all", batch: 2, hidden: true,
                           },
                           {
                               id: "TB2_V1", header: ["TB2_V1"], template: function (obj) {
                                   return round(obj.TB2_V1, 2);
                               }, adjust: "all", batch: 2, hidden: true,
                           },
                           {
                               id: "TB2_V2", header: ["TB2_V2"], template: function (obj) {
                                   return round(obj.TB2_V2, 2);
                               }, adjust: "all", batch: 2, hidden: true,
                           },
                           {
                               id: "TB2_V3", header: ["TB2_V3"], template: function (obj) {
                                   return round(obj.TB2_V3, 2);
                               }, adjust: "all", batch: 2, hidden: true,
                           },
                           {
                               id: "TB2_P1", header: ["TB2_P1"], template: function (obj) {
                                   return round(obj.TB2_P1, 2);
                               }, adjust: "all", batch: 2, hidden: true,
                           },
                           {
                               id: "TB2_P2", header: ["TB2_P2"], template: function (obj) {
                                   return round(obj.TB2_P2, 2);
                               }, adjust: "all", batch: 2, hidden: true,
                           },
                           {
                               id: "TB2_Q", header: ["TB2_Q"], adjust: "all", batch: 2, hidden: true, template: function (obj) {
                                   return round(obj.TB2_Q, 2);
                               },
                           },
                           {
                               id: "TB2_Ti", header: ["TB2_Ti"], template: function (obj) {
                                   return round(obj.TB2_Ti, 2);
                               }, adjust: "all", batch: 2, hidden: true,
                           },

                           {
                               id: "TB2_Qg", header: ["TB2_Qg"], adjust: "all", batch: 2, hidden: true, template: function (obj) {
                                   return round(obj.TB2_Qg, 2);
                               },
                           },
                       ],
                       on: {
                           "onItemDblClick": webix.once(function (id, e, node) {
                               this.getTopParentView().hide();
                           }),
                           "onLoadError": webix.once(function () {
                               this.hide();
                               this.adjust();
                           }),
                           "onBeforeLoad": function () {
                               this.showOverlay("Загрузка...");
                               this.show();
                           },
                           "onAfterLoad": function () {
                               this.hideOverlay();
                               if (!this.count()) {
                                   this.showOverlay("<b>Нет данных за указанный период</b>");
                                   this.hide();
                               }
                               this.adjust();

                           },


                       },

                       autowidth: true,
                       //  autoheight: true,
                       //headermenu:true


                   }
            ],

        }
    }).show();
    webix.extend($$("garchives"), webix.ProgressBar);
    PrepareArchive();
    webix.i18n.setLocale("ru-RU");
}
function ShowWindowArchivesIzm(id) {

    PM = GetPM(id);
    var address = PM.properties.get('address');
    idMD = PM.properties.get('idMD');

    webix.ui.datafilter.countRows = webix.extend({
        refresh: function (master, node, value) {
            node.firstChild.innerHTML = master.count();
        }
    }, webix.ui.datafilter.summColumn);
    webix.i18n.setLocale("ru-RU");
    webix.ui({
        view: "window", id: "wizmarchives", left: 50, top: 50, move: true, resize: true,
        head: {
            view: "toolbar", cols: [
                            { view: "label", label: "Таблица архивов по адресу: " + address },

                            {
                                view: "button", label: 'Excel', width: 100, align: 'right', click: function () {
                                    webix.toExcel($$("gizmarchives"));
                                }
                            },
                                        { view: "button", label: 'Закрыть', width: 100, align: 'right', click: function () { this.getTopParentView().close(); } },

            ]
        },
        body: {
            rows: [
                       {
                           cols: [ 
                                 { view: "button", value: "Получить", width: 150, click: "GetArchiveIZM" },
                                 { view: "button", id: "startArc", value: "Опрос", width: 150, click: "StartProccessArc" },
                           ]
                       },



                   {
                       view: "datatable",
                       id: "gizmarchives", width: 800, height: 400, select: "row", scrollX: true,

                       columns: [
                           {
                               id: "Date", header: "Дата", fillspace: true,
                               template: function (obj) {
                                   var date = moment(obj.Date);
                                   return date.format("DD.MM.YYYY HH:mm");
                               },
                               width: 160, sort: "date", format: webix.i18n.dateFormatStr
                           },
                          
                           {
                               id: "TB", header: ["TB",{content:"selectFilter"}], width: 110, fillspace: true
                           },
                           {
                               id: "message", header: ["Сообщение", { content: "selectFilter" }], width: 300, fillspace: true
                           },
                       ],
                       on: {
                           "onLoadError": webix.once(function () {
                               this.hide();
                           }),
                           "onBeforeLoad": function () {
                               this.showOverlay("Загрузка...");
                           },
                           "onAfterLoad": function () {
                               this.hideOverlay();
                               if (!this.count())
                                   this.showOverlay("Нет данных за указанный период");
                           },

                       },

                       autowidth: true,
                       //headermenu:true


                   }
            ],

        }
    }).show();
    webix.extend($$("gizmarchives"), webix.ProgressBar);
    GetArchiveIZM();
    webix.i18n.setLocale("ru-RU");
}
function GetArchiveIZM() {
    PM = GetPM(pmid);
    idMD = PM.properties.get('idMD');
    var url = 'ukut.asmx/GetIzmArc?KPNum=' + idMD;
    $$("gizmarchives").clearAll();
    $$("gizmarchives").load(url);
}
function StartProccessArc()
{	
	//PM = GetPM(pmid);
    idMD = $$("idMD").getValue();
	var params = '?KPNum='+idMD;
	AJAX('ukut.asmx/StartProccessArc'+params, params, function(){
		if (xmlHttp.readyState == 4 || xmlHttp.readyState == "complete") {
			//alert(xmlHttp.responseText);
			$$('startArc').disable();
		}
	});
}
function ShowRegulatScheme()
{
	webix.ui.datafilter.countRows = webix.extend({
	refresh:function(master, node, value){
    node.firstChild.innerHTML = master.count();
  }
}, webix.ui.datafilter.summColumn);
	
webix.ui({
	view:"window",  id:"uregular",  width:397,  hight:198,
    head:{
		view:"toolbar", cols:[
						{view:"label", label: "Управление регулятором" },
						{ view:"button", label: 'Закрыть', width:100, align: 'right', click:function(){this.getTopParentView().close();}},
						
						]
	},
	body:{ 	template:img, data: { src:"img/scheme/trm2101.svg" }
	},
	
}).show();
	function img(obj){
		return '<object  id="regulator_svg" data="'+obj.src+'" type="image/svg+xml"></object>';
	}
			 var PM = GetPM(pmid);
			 var con = PM.properties.get('balloonContentBody');
			 var SVG = document.getElementById("regulator_svg").contentDocument;
			 var delta = SVG.getElementById("btnup");
			 console(delta);
}