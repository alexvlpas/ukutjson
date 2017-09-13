
        //var Monday = moment().startOf('isoweek').isoWeekday(1).format("YYYY.MM.DD");
        var Monday = moment().format("YYYY.MM.DD");
webix.i18n.setLocale("ru-RU");
webix.ready(function () {
           
    var flex = {
        margin: 10, padding: 0, type: "wide",
        view: "accordion",
        rows: [
            listFilter,
            {   
                cols: [
                        {
                            header: "Объекты", body: { rows: [radio, listUkut] }
                        
                        },
                            
                        {
                            view: "resizer"
                               
                        },
              { header: "Текущие значения", body: current_table, collapsed: true },
              { header: "События с начала недели", body: tableEvents,  },
              { header: "Описание объекта", body: propertysheet_1, },
                ]
            },
                  
                    
        ],
        on: {
            "onAfterCollapse": function () {
                alert("111");
            }
        },

    };
       
    webix.ui(flex);
    webix.ui.fullScreen();
    webix.extend($$("current_table"), webix.ProgressBar);
    webix.extend($$("listB"), webix.ProgressBar);
    webix.extend($$("testA"), webix.ProgressBar);
    $$("listB").load("/ukut.asmx/GetObjectListEventCount?SelectDate=" + Monday);
    $$("list_input").attachEvent("onTimedKeyPress", function () {
        var value = this.getValue().toLowerCase();
        $$("listB").filter(function (obj) {
            return obj.address.toLowerCase().indexOf(value) == 0;
        })
    });

})
var radio = {
    view: "radio",
    label: "Выбор по:",
    value: 1, options: [
    { id: 1, value: "Абвг" },
    { id: 2, value: "Кол-ву событий" }],
    on: {
        "onChange": function (newv, oldv) {
            if(newv == 1)
                $$("listB").sort("address");
            if (newv == 2)
                $$("listB").sort("CountTrabl", "desc", "int");
        }
    },

};
var listFilter = {
    height: 35,
    view: "toolbar",
    elements: [
    { view: "text", id: "list_input", label: "Поиск по адресу", css: "fltr", labelWidth: 150, width:500 },
    { view: "button", value: "ToExcel", width:60,  on: {
        "onItemClick": function (id, e, node) {
            webix.toExcel($$("testA"));
        }, 
    },
    },
            {
                view: "button", value: "На карте",width:80, on: {
                    "onItemClick": function (id, e, node) {
                        var item = $$("listB").getSelectedItem();
                        document.location.assign("/?idUkut=" + item.idUkut);
                    },
                },
            },
             {
                 view: "button", value: "Архивы", width: 80, on: {
                     "onItemClick": function (id, e, node) {
                         GetAtchive();
                         $$("warchives").show();
                     },
                 },
             },
             {
                 view: "datepicker", id: "dateEvents", timepicker: true, width: 200,
                 label: "Накоплено с: ", value: Monday,
                 name: "start", stringResult: true, format: "%d  %M %Y",
                 on: {
                     "onChange": function () {
                         Monday = $$("dateEvents").getValue();
                         $$("listB").clearAll();
                         $$("listB").load('/ukut.asmx/GetObjectListEventCount?SelectDate=' + Monday);
                     },

                 },
             },
    ]
};

var listUkut = {
    view: "list",
    width: 320,
    id: "listB",
    load:"/ukut.asmx/GetObjectListEventCount?SelectDate="+Monday, 
    sort:{
        by:"#address#",
        dir:"asc",
        as:"string"
    },
    ready: function () {
        if (!this.count()) {
            webix.extend(this, webix.OverlayBox);
            this.showOverlay("<div style='margin:75px; font-size:20px;'>Нет данных!</div>");
        }
    },
    template: "<div class='mark'>#CountTrabl# </div> #address#",
    type: {
        height: 62
    },
    select: true,
    on: {
        "onItemClick": function (id, e, node) {
            var item = this.getItem(id);
            $$("testA").clearAll();
            $$("testA").load("/ukut.asmx/EventFromKpNum?KPNum=" + item.idMD+"&SelectDate="+Monday);
            $$("current_table").clearAll();
            $$("current_table").load("/ukut.asmx/GetCurrentValues?idUkut="+item.idUkut+"&KPNum=" + item.idMD);
            $$("sets").setValues({
                address: item.address,
                devName: item.devName + ' № ' + item.serialNum,
                geu: item.geu,
                teplo_source: item.teplo_source,
                countTrub: item.countTrub,
                system: item.system,
                HID: item.HID

            });
        },
        "OnAfterLoad": function (id, e, node) {
            this.select(this.getFirstId());
            var item = this.getSelectedItem();
            $$("testA").clearAll();
            $$("testA").load("/ukut.asmx/EventFromKpNum?KPNum=" + item.idMD + "&SelectDate=" + Monday);
            $$("current_table").clearAll();
            $$("current_table").load("/ukut.asmx/GetCurrentValues?idUkut=" + item.idUkut + "&KPNum=" + item.idMD);
            $$("sets").setValues({
                address: item.address,
                devName: item.devName +' № '+ item.serialNum,
                geu: item.geu,
                teplo_source: item.teplo_source,
                countTrub: item.countTrub,
                system: item.system,
                HID: item.HID

            });
        },
    },

           
};
var current_table = {
    id: "current_table",
    view: "treetable",
    autowidth: true,
    autowidth: true,
    resizeColumn: true,
    select: "row",
    //width: 620,
    //height: 600,
    columns: [
  
    { id: "channel_name", header: "Параметр", width: 280, },
    { id: "channel_value", header: "Значение", width: 130 },
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
        },

    },
};
var tableEvents = {
    id:"testA",
    view: "treetable",
    autowidth: true,
    autowidth: true,
    resizeColumn: true,
    select: "row",
    columns: [
    {
        id: "datetime", header: "Дата", fillspace: true,
        template: function (obj, common) {
            var date = moment(obj.datetime);
            if (obj.$level == 1)
                return " ";
            return date.format("DD.MM.YYYY HH:mm:ss");
        }, width: 130, sort: "date", format: webix.i18n.dateFormatStr
    },
    {
        id: "ParamName", header: "Параметр", width: 280, template: function (obj, common) {
            if (obj.$level == 1) return common.treetable(obj, common) + obj.ParamName;
            return obj.descr;
        }, exportAsTree: true, 
    },
    { id: "OldStates", header: "Пред. состояние", width: 130 },
    { id: "oldcnlval", header: "Пред. значение", width: 60 },
    { id: "NewStates", header: "Нов. состояние", width: 130 },
    { id: "newcnlval", header: "Нов. значение", width: 60 },
    ],
    on: {
        "onAfterSelect": function (data, preserve) {
            // $$('chart_repair').data = data;
        },
        "onAfterLoad": function (data, preserve) {
            this.openAll();
        },
    },
    scheme: {
        $group: "ParamName",
    },
};
var system_options = [
    { id: "false", value: "Открытая" },
    { id: "true", value: "Закрытая" }
];
var propertysheet_1 = {
    view: "property", id: "sets", width: 400, editable: false,
    elements: [
        { label: "Объект учета", type: "label" },
        { label: "Адрес", type: "text", id: "address"},
        { label: "Прибор", type: "text", id: "devName" },
        { label: "ЖЭУ", type: "text", id: "geu" },
        { label: "Код дома", type: "text", id: "HID" },
        { label: "Характеристика системы", type: "label" },
        { label: "Кол-во труб", type: "text", id: "countTrub"},
        { label: "Система", type: "select", options: system_options, id: "system" },
    ]
};
var chart_repair = {
    view: "chart",
    type: "spline",
    width: 320,
    id:"chart_repair",
    value: "#ParamName#",
    item: {
        borderColor: "#1293f8",
        color: "#ffffff"
    },
    line: {
        color: "#1293f8",
        width: 3
    },
    offset: 0,
    yAxis: {
        start: 0,
        end: 100,
        step: 10,
        template: function (obj) {
            //   return (obj % 20 ? "" : obj)
        }
    },
    //data: dataset
};
webix.ui({
    view: "window", id: "warchives", left: 50, top: 50, move: true, resize: true,
    head: {
        view: "toolbar", cols: [
                        { view: "label", id: "address_label", label: "Таблица архивов по адресу: " },

                        {
                            view: "button", label: 'Excel', width: 100, align: 'right', click: function () {
                                webix.toExcel($$("garchives"));
                            }
                        },
                                    { view: "button", label: 'Закрыть', width: 100, align: 'right', click: function () { this.getTopParentView().hide(); } },

        ]
    },
    body: {
        rows: [
                   {
                       cols: [ //2nd row
                         {
                             rows: [
                                { view: "radio", customRadio: false, name: "arc", id: "arc", label: "Архив: ", value: 2, options: [{ id: 1, value: "Часовые" }, { id: 2, value: "Суточные" }] },
                                { view: "datepicker", id: "dateFrom", timepicker: true, width: 300, label: "От", value: moment().startOf('month').format("YYYY.MM.DD HH:mm"), name: "start", stringResult: true, format: "%d  %M %Y %H:%i" },
                                { view: "datepicker", id: "dateTo", suggest: { type: "calendar", body: { maxDate: new Date() } }, value: moment().format("YYYY.MM.DD HH:mm"), timepicker: true, width: 300, label: "До", name: "end", stringResult: true, format: "%d %M %Y %H:%i" },
                             ]
                         },
                             { view: "button", value: "Получить", width: 150, click: "GetAtchive" },
                             { view: "button", id: "startArc", value: "Опрос", width: 150, click: "StartProccessArc" },
                       ]
                   },



               {
                   view: "datatable",
                   id: "garchives", width: 800, height: 400, select: "row", scrollX: true,

                   columns: [
                       {
                           id: "Date", header: "Дата", fillspace: true,
                           template: function (obj) {
                               var date = moment(obj.Date);
                               return date.format("DD.MM.YYYY HH:mm");
                           },
                           width: 160, sort: "date", format: webix.i18n.dateFormatStr
                       },
                       //TB1
                       {
                           id: "TB1_M1", header: ["M1, т"], template: function (obj) {
                               return round(obj.TB1_M1, 2);
                           }, width: 110, fillspace: true
                       },
                       {
                           id: "TB1_M2", header: ["M2, т"], template: function (obj) {
                               return round(obj.TB1_M2, 2);
                           }, width: 110, fillspace: true
                       },
                       {
                           id: "TB1_M3", header: ["M3, т"], template: function (obj) {
                               return round(obj.TB1_M3, 2);
                           }, width: 110, fillspace: true, hidden: true
                       },
                       {
                           id: "TB1_t1", header: ["t1, C"], template: function (obj) {
                               return round(obj.TB1_t1, 2);
                           }, width: 110, fillspace: true
                       },
                       {
                           id: "TB1_t2", header: ["t2, C"], template: function (obj) {
                               return round(obj.TB1_t2, 2);
                           }, width: 110, fillspace: true
                       },
                       {
                           id: "TB1_t3", header: ["t3, C"], template: function (obj) {
                               return round(obj.TB1_t3, 2);
                           }, width: 110, fillspace: true, hidden: true
                       },
                       {
                           id: "TB1_tx", header: ["tx, C"], template: function (obj) {
                               return round(obj.TB1_tx, 2);
                           }, width: 110, fillspace: true, hidden: true
                       },
                       {
                           id: "TB1_tv", header: ["tv, C"], template: function (obj) {
                               return round(obj.TB1_tv, 2);
                           }, width: 110, fillspace: true, hidden: true
                       },
                       {
                           id: "TB1_V1", header: ["V1, м3"], template: function (obj) {
                               return round(obj.TB1_V1, 2);
                           }, width: 110, fillspace: true, hidden: true
                       },
                       {
                           id: "TB1_V2", fillspace: true, header: ["V2, м3"], template: function (obj) {
                               return round(obj.TB1_V2, 2);
                           }, width: 110, fillspace: true, hidden: true
                       },
                       {
                           id: "TB1_V3", header: ["V3, м3"], template: function (obj) {
                               return round(obj.TB1_V3, 2);
                           }, width: 110, fillspace: true, hidden: true
                       },
                       {
                           id: "TB1_P1", header: ["P1, ат"], template: function (obj) {
                               return round(obj.TB1_P1, 2);
                           }, width: 110, fillspace: true
                       },
                       {
                           id: "TB1_P2", header: ["P2, ат"], template: function (obj) {
                               return round(obj.TB1_P2, 2);
                           }, width: 110, fillspace: true
                       },
                       {
                           id: "TB1_Q", header: ["Q, Гкал"], template: function (obj) {
                               return round(obj.TB1_Q, 2);
                           }, width: 110, hidden: true, fillspace: true
                       },
                       {
                           id: "TB1_Ti", header: ["Ti"], template: function (obj) {
                               return round(obj.TB1_Ti, 2);
                           }, width: 110, hidden: true, fillspace: true
                       },
                       //TB2
                       {
                           id: "TB2_M1", header: ["M1, т"], template: function (obj) {
                               return round(obj.TB2_M1, 2);
                           }, width: 110, fillspace: true
                       },
                       {
                           id: "TB2_M2", header: ["M2, т"], template: function (obj) {
                               return round(obj.TB2_M2, 2);
                           }, width: 110, fillspace: true
                       },
                       {
                           id: "TB2_M3", header: ["M3, т"], template: function (obj) {
                               return round(obj.TB2_M3, 2);
                           }, width: 110, hidden: true, fillspace: true
                       },
                       {
                           id: "TB2_t1", header: ["t1, C"], template: function (obj) {
                               return round(obj.TB2_t1, 2);
                           }, width: 110, fillspace: true
                       },
                       {
                           id: "TB2_t2", header: ["t2, C"], template: function (obj) {
                               return round(obj.TB2_t2, 2);
                           }, width: 110, fillspace: true
                       },
                       {
                           id: "TB2_t3", header: ["t3, C"], template: function (obj) {
                               return round(obj.TB2_t3, 2);
                           }, width: 110, hidden: true, fillspace: true
                       },
                       {
                           id: "TB2_t3", header: ["tx, C"], template: function (obj) {
                               return round(obj.TB2_tx, 2);
                           }, width: 110, hidden: true, fillspace: true
                       },
                       {
                           id: "TB2_tv", header: ["tv, C"], template: function (obj) {
                               return round(obj.TB2_tv, 2);
                           }, width: 110, hidden: true, fillspace: true
                       },
                       {
                           id: "TB2_V1", header: ["V1, м3"], template: function (obj) {
                               return round(obj.TB2_V1, 2);
                           }, width: 110, hidden: true, fillspace: true
                       },
                       {
                           id: "TB2_V2", header: ["V2, м3"], template: function (obj) {
                               return round(obj.TB2_V2, 2);
                           }, width: 110, hidden: true, fillspace: true
                       },
                       {
                           id: "TB2_V3", header: ["V3, м3"], template: function (obj) {
                               return round(obj.TB2_V3, 2);
                           }, width: 110, hidden: true, fillspace: true
                       },
                       {
                           id: "TB2_P1", header: ["P1, ат"], template: function (obj) {
                               return round(obj.TB2_P1, 2);
                           }, width: 110, fillspace: true
                       },
                       {
                           id: "TB2_P2", header: ["P2, ат"], template: function (obj) {
                               return round(obj.TB2_P2, 2);
                           }, width: 110, fillspace: true
                       },
                       {
                           id: "TB2_Ti", header: ["Ti"], template: function (obj) {
                               return round(obj.TB2_Ti, 2);
                           }, width: 110, hidden: true, fillspace: true
                       },
                       { id: "TB2_Q", header: ["TB2 Qгвс"], width: 110, fillspace: true, hidden: true },
                   ],
                   on: {
                       "onItemDblClick": webix.once(function (id, e, node) {
                           var item = this.getItem(id);
                           ZoomIn(item.idUkut);
                           this.getTopParentView().hide();
                       }),
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
                           var item = $$("listB").getSelectedItem();
                           $$("address_label").setValue("Таблица архивов по адресу: "+item.address);
                       },

                   },

                   autowidth: true,
               }
        ],

    }
});
function GetAtchive() {
    var item = $$("listB").getSelectedItem();
         
    var arc = $$("arc").getValue();
    if (arc == 2) {
        arc = "days";
    }
    else {
        arc = "hours"
    }
    var dateFrom = $$("dateFrom").getValue();
    var dateTo = $$("dateTo").getValue();
    var url = '../ukut.asmx/GetArchive?arc=' + arc + '&idMD=' + item.idMD + '&dateFirst="' + dateFrom + '"&dateLast="' + dateTo + '"';
    $$("garchives").clearAll();
    $$("garchives").load(url);
}
function round(a, b) {
    b = b || 0;
    return Math.round(a * Math.pow(10, b)) / Math.pow(10, b);
}
function StartProccessArc() {
    var item = $$("listB").getSelectedItem();
    idMD = item.idMD;
    var params = '?KPNum=' + idMD;
    AJAX('../rsapi.asmx/StartProccessArc' + params, params, function () {
        if (xmlHttp.readyState == 4 || xmlHttp.readyState == "complete") {
            //alert(xmlHttp.responseText);
            $$('startArc').disable();
        }
    });
}
function AJAX(url, params, OnChange) {
    xmlHttp = GetXmlHttpObject(); if (xmlHttp == null) return;
    if (OnChange != null) xmlHttp.onreadystatechange = OnChange;
    xmlHttp.open('GET', url, true);
    xmlHttp.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
    xmlHttp.send(params);
}
function GetXmlHttpObject() {
    var xmlHttp;
    try { xmlHttp = new ActiveXObject('Msxml2.XMLHTTP'); }
    catch (e) {
        try { xmlHttp = new ActiveXObject('Microsoft.XMLHTTP'); }
        catch (E) { xmlHttp = false; }
    }
    if (!xmlHttp && typeof XMLHttpRequest != 'undefined') { xmlHttp = new XMLHttpRequest(); }
    return xmlHttp;
}