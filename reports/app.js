function GetKartochka() {

    /* var dateFrom = $$("dateFromKartochka").getValue();
     var dateTo = $$("dateToKartochka").getValue();
 
     var border = 0;
     var url = '/ukut.asmx/GetReportToSTS?arc=' + arc + '&dateFirst="' + dateFrom + '"&dateLast="' + dateTo + '"&border=' + border + "&countTrub=" + countTrub;
     window.location = url;
     $$("ButtonGetSTK").disable();
     */
    
   
    var objects = [];
    $$("objectsList").data.each(function (obj) {
        if ("markCheckbox" in obj) {
            if (obj.markCheckbox == 1)
            {
                objects.push(obj.idMD);
            }
        }
    });

    if(objects.length > 0)
    {
    var dateFrom = $$("dateFromKartochka").getValue();
    var dateTo = $$("dateToKartochka").getValue();
    var url = '/ukut.asmx/GetTeploReport?dateFirst="' + dateFrom + '"&dateLast="' + dateTo + '"&objects=' + objects.join(",");
    window.location = url;
    }
    else {
        webix.message({
            type: "error",
            text: "Не выбран ни один объект",
            expire: 1000
            //expire:-1   for canceling expire period
        });
    }
}
function GetTest() {
    $$("objectsList").data.each(function (obj)
    {
        // titles.push(obj.title);
        obj.markCheckbox = 0;
    });
    $$("objectsList").refresh();
}
function GetAnaliz() {
    var dogovor;

    var objects = [];
    $$("objectsListSTS").data.each(function (obj) {
        if ("markCheckbox" in obj) {
            if (obj.markCheckbox == 1) {
                objects.push(obj.idMD);
                dogovor = obj.dogovor;
            }
        }
    });

    if (objects.length > 0) {
        var arc = $$("arcSTK").getValue();
        if (arc == 2) {
            arc = "days";
        }
        else {
            arc = "hours"
        }
        var dateFrom = $$("dateFromSTK").getValue();
        var dateTo = $$("dateToSTK").getValue();
        var countTrub = $$("countTrub").getValue();
        var border = 0;
        var url = '/ukut.asmx/GetReportToSTS?arc=' + arc + '&dateFirst="' + dateFrom + '"&dateLast="' + dateTo + '"&border=' + border + "&countTrub=" + countTrub+"&objects="+objects.join(",")+"&dogovor="+dogovor;
        window.location = url;
        $$("ButtonGetSTK").disable();
    }
    else {
        webix.message({
            type: "error",
            text: "Не выбран ни один объект",
            expire: 1000
            //expire:-1   for canceling expire period
        });
    }  
}
function GetAnalizPeretops() {
   

    var objects = [];
    $$("objectsListSTS").data.each(function (obj) {
        if ("markCheckbox" in obj) {
            if (obj.markCheckbox == 1) {
                objects.push(obj.idMD);
            }
        }
    });

    if (objects.length > 0) {
        var arc = $$("arcPeretop").getValue();
        if (arc == 2) {
            arc = "days";
        }
        else {
            arc = "hours"
        }
        var dateFrom = $$("dateFromPeretop").getValue();
        var dateTo = $$("dateToPeretop").getValue();
        var border = 0;
        var url = '/ukut.asmx/GetReportPeretops?arc=' + arc + '&dateFirst="' + dateFrom + '"&dateLast="' + dateTo + '"&border=' + border + "&objects=" + objects.join(",");
        window.location = url;
        $$("ButtonGetPeretop").disable();
    }
    else {
        webix.message({
            type: "error",
            text: "Не выбран ни один объект",
            expire: 1000
        });
    }




}
var views = {
    id: "main", visibleBatch: "ReportToSTS", rows: [
        {
            view: "toolbar",
            elements: [
            { view: "label", id: "header", label: "Настройте отчет" },
            {},
            ]
        },
        
         {
             view: "form",
             batch: "ReportToSTS",
             elements: [
               { view: "radio", customRadio: false, name: "arc", id: "arcSTK", label: "Архив: ", value: 1, options: [{ id: 1, value: "Часовые" }, { id: 2, value: "Суточные" }] },
                    { view: "datepicker", id: "dateFromSTK", timepicker: true, icons: true, width: 300, label: "От", value: moment().startOf('month').format("YYYY.MM.DD HH:mm"), name: "start", stringResult: true, format: "%d  %M %Y %H:%i" },
                    { view: "datepicker", id: "dateToSTK", timepicker: true, icons: true, value: moment().format("YYYY.MM.DD 23:00"), width: 300, label: "До", name: "end", stringResult: true, format: "%d %M %Y %H:%i" },
                    { view: "text", id: "countTrub", value: '2', label: "Труб >=:" },
                    {
                                                       view: "activeList",
                                                       id: "dogovorListSTS",
                                                       height: 110,
                                                       select: true,
                                                       hide: true,
                                                       url: "/ukut.asmx/GetDogovorList",
                                                       template: "Договор №: #dogovor#",
                                                       on: {
                                                           "onItemClick": function (id, e, node) {
                                                               var item = this.getItem(id);
                                                               //alert(item.dogovor);
                                                               $$("objectsListSTS").filter('#dogovor#', item.dogovor);
                                                           }

                                                       },
                     },
                     {
                                        view: "checkbox", labelWidth: 180, label: "Отчет по всем объектам", value: 1,
                                        on: {
                                            "onChange": function (newv, oldv) {
                                                if (newv == 0) {
                                                    $$("objectsListSTS").data.each(function (obj) {
                                                        // titles.push(obj.title);
                                                        obj.markCheckbox = 0;
                                                    });
                                                    $$("objectsListSTS").refresh();
                                                }
                                                if (newv == 1) {
                                                    $$("objectsListSTS").data.each(function (obj) {
                                                        // titles.push(obj.title);
                                                        obj.markCheckbox = 1;
                                                    });
                                                    $$("objectsListSTS").refresh();
                                                }
                                            },

                                        },
                                    },
                    {
                        rows: [

                             {
                                 height: 35,
                                 view: "toolbar",
                                 hide: true,
                                 id: "tools_filter_STS",
                                 elements: [
                                  {
                                      view: "text", id: "filter_list_STS", label: "Фильтр по адресу", css: "fltr", labelWidth: 170,
                                  }
                                 ]
                             },
        
                                 {
                                     view: "activeList",
                                     id: "objectsListSTS",
                                     width: 650,
                                     select: true,
                                     hide: true,
                                     url: "/ukut.asmx/GetObjectListForReports",
                                     type: {
                                         markCheckbox: function (obj) {
                                             return "<span class='check webix_icon fa-" + (obj.markCheckbox ? "check-" : "") + "square-o'></span>";
                                         }
                                     },
                                     onClick: {
                                         "check": function (e, id) {
                                             var item = this.getItem(id);
                                             item.markCheckbox = item.markCheckbox ? 0 : 1;
                                             this.updateItem(id, item);
                                         }
                                     },
                                     template: "#address#{common.markCheckbox()}",


                                 },
                          ]
                      },
                    { view: "button", id: "ButtonGetSTK", value: "Получить", click: "GetAnaliz" },
             ]
         },

          {
              view: "form",
              batch: "ReportGvsPeretop",
              elements: [
                { view: "radio", customRadio: false, name: "arc", id: "arcPeretop", label: "Архив: ", value: 1, options: [{ id: 1, value: "Часовые" }, { id: 2, value: "Суточные" }] },
                     { view: "datepicker", id: "dateFromPeretop", timepicker: true, icons: true, width: 300, label: "От", value: moment().startOf('month').format("YYYY.MM.DD HH:mm"), name: "start", stringResult: true, format: "%d  %M %Y %H:%i" },
                     { view: "datepicker", id: "dateToPeretop", timepicker: true, icons: true, value: moment().format("YYYY.MM.DD 23:00"), width: 300, label: "До", name: "end", stringResult: true, format: "%d %M %Y %H:%i" },
                       {
                           view: "checkbox", labelWidth: 180, label: "Отчет по всем объектам", value: 1,
                           on: {
                               "onChange": function (newv, oldv) {
                                   if (newv == 0) {
                                       $$("objectsListPerepop").data.each(function (obj) {
                                           // titles.push(obj.title);
                                           obj.markCheckbox = 0;
                                       });
                                       $$("objectsListPerepop").refresh();
                                   }
                                   if (newv == 1) {
                                       $$("objectsListPerepop").data.each(function (obj) {
                                           // titles.push(obj.title);
                                           obj.markCheckbox = 1;
                                       });
                                       $$("objectsListPerepop").refresh();
                                   }
                               },

                           },
                       },

                             {
                                 rows: [
                                    {
                                        height: 35,
                                        view: "toolbar",
                                        hide: true,
                                        id: "tools_filter_Perepop",
                                        elements: [
                                         {
                                             view: "text", id: "filter_list_Perepop", label: "Фильтр по адресу", css: "fltr", labelWidth: 170,
                                         }
                                        ]
                                    },
                                        {
                                            view: "activeList",
                                            id: "objectsListPerepop",
                                            width: 650,
                                            select: true,
                                            hide: true,
                                            url: "/ukut.asmx/GetObjectListForReports",
                                            type: {
                                                markCheckbox: function (obj) {
                                                    return "<span class='check webix_icon fa-" + (obj.markCheckbox ? "check-" : "") + "square-o'></span>";
                                                }
                                            },
                                            onClick: {
                                                "check": function (e, id) {
                                                    var item = this.getItem(id);
                                                    item.markCheckbox = item.markCheckbox ? 0 : 1;
                                                    this.updateItem(id, item);
                                                }
                                            },
                                            template: "#address#{common.markCheckbox()}",

                                        },
                                 ]
                             },
                     { view: "button", id: "ButtonGetPeretop", value: "Получить", click: "GetAnalizPeretops" },
              ]
          },
          {
              view: "form",
                      batch: "ReportKartochka",
                      elements: [
                             { view: "datepicker", id: "dateFromKartochka", timepicker: true, icons: true, width: 300, label: "От", value: moment().startOf('month').format("YYYY.MM.DD HH:mm"), name: "start", stringResult: true, format: "%d  %M %Y %H:%i" },
                             { view: "datepicker", id: "dateToKartochka", timepicker: true, icons: true, value: moment().format("YYYY.MM.DD 23:00"), width: 300, label: "До", name: "end", stringResult: true, format: "%d %M %Y %H:%i" },
                             {
                                 view: "checkbox", labelWidth:180, label: "Отчет по всем объектам", value: 1,
                                 on: {
                                     "onChange": function (newv, oldv)
                                     {
                                         if(newv ==0)
                                         {
                                             $$("objectsList").data.each(function (obj) {
                                                 // titles.push(obj.title);
                                                 obj.markCheckbox = 0;
                                             });
                                             $$("objectsList").refresh();
                                         }
                                         if (newv == 1) {
                                             $$("objectsList").data.each(function (obj) {
                                                 // titles.push(obj.title);
                                                 obj.markCheckbox = 1;
                                             });
                                             $$("objectsList").refresh();
                                         }
                                     },

                                 },
                             },

                             {
                                 rows: [
                                    {
                                    height: 35,
                                    view: "toolbar",
                                    hide: true,
                                   id:"tools_filter",
                                elements: [
                                 {
                                     view: "text", id: "filter_list", label: "Фильтр по адресу", css: "fltr", labelWidth: 170,
                                 }
                                ]
                                 },
                                        {
                                            view: "activeList",
                                            id: "objectsList",
                                            width: 650,
                                            select: true,
                                            hide: true,
                                            url: "/ukut.asmx/GetObjectListForReports",
                                            type: {
                                                markCheckbox: function (obj) {
                                                    return "<span class='check webix_icon fa-" + (obj.markCheckbox ? "check-" : "") + "square-o'></span>";
                                                }
                                            },
                                            onClick: {
                                                "check": function (e, id) {
                                                    var item = this.getItem(id);
                                                    item.markCheckbox = item.markCheckbox ? 0 : 1;
                                                    this.updateItem(id, item);
                                                }
                                            },
                                            template: "#address#{common.markCheckbox()}",
    
                                        },
                                 ]
                             },
                                  { view: "resizer" },
                             { view: "button", id: "ButtonGetKartochka", value: "Получить", click: "GetKartochka" },
                           //  { view: "button", id: "Buttontest", value: "Test", click: "GetTest" },
                      ]
          },


    ]
};
var menu = {


    rows: [
        { type: "header", template: "Выберите отчет" },
        {
            view: "list", id: "sidemenu", scroll: false, layout: "y", template: "#value#", select: true, width: 230,
            data: [
                { id: "ReportToSTS", value: "Недопоставки в СТК", options: [] },
              //  { id: "ReportGvsPeretop", value: "По перетопам для ЖЭУ", options: [] },
                { id: "ReportKartochka", value: "Отчетные карточки в СТК", options: [] },
            ],
            on: {
                onItemClick: function (id)
                {
                    $$("main").showBatch(id);
                }
            }
        }
    ]
};
var ui = {
    rows: [
        {
            view: "toolbar", height: 50, elements: [
               { view: "label", label: "<div style = 'font-size:18px; margin-right:5px; color:#fff;'>Отчеты телеметрия</div>" }
            ]
        },

        { type: "space", cols: [menu, views] }
    ]
};

webix.ready(function () {
    webix.protoUI({
        name: "activeList"
    }, webix.ui.list, webix.ActiveContent);

    webix.i18n.setLocale("ru-RU");
  
    webix.ui(ui);
    $$("sidemenu").select("ReportToSTS");
    $$("filter_list").attachEvent("onTimedKeyPress", function () {
        var value = this.getValue().toLowerCase();
        $$("objectsList").filter(function (obj) {
            return obj.address.toLowerCase().indexOf(value) == 0;
        })
    });
    $$("filter_list_STS").attachEvent("onTimedKeyPress", function () {
        var value = this.getValue().toLowerCase();
        $$("objectsListSTS").filter(function (obj) {
            return obj.address.toLowerCase().indexOf(value) == 0;
        })
    });
    $$("objectsList").attachEvent("onKeyPress", function (code, e)
    {
        if(code === 32)
        {
            var id = $$("objectsList").getSelectedId();
            var item = $$("objectsList").getSelectedItem();
            item.markCheckbox = item.markCheckbox ? 0 : 1;
            $$("objectsList").updateItem(id, item);
        }
    });
});
