function ShowWindowUkuts()
{
    webix.i18n.setLocale("ru-RU");
    webix.ui.datafilter.countRows = webix.extend({
        refresh: function (master, node, value) {
            node.firstChild.innerHTML = master.count();
        }
    }, webix.ui.datafilter.summColumn);

    webix.ui({
        view: "window", id: "utickets", move: true, resize: true, fullscreen: true,
        head: {
            view: "toolbar", cols: [
                            { view: "label", label: "Таблица объектов диспетчерской связи" },

                            {
                                view: "button", label: 'Excel', width: 100, align: 'right', click: function () {
                                    webix.toExcel($$("grid_ukuts"));
                                }
                            },
                                { view: "button", label: 'Закрыть', width: 100, align: 'right', click: function () { this.getTopParentView().close(); } },
            ]
        },
        body: {
            rows: [
     
                   {
                       view: "treetable",
                       id: "grid_ukuts", select: "row",

                       columns: [
                           { id: "idUkut", header: "#", width: 50 },
                           { id: "address", header: ["Адрес", { content: "textFilter" }], width: 200, sort: "string", footer: { text: "Всего:", colspan: 3 } },
                           { id: "geu", header: ["ЖЭУ", { content: "selectFilter" }], width: 50, sort: "int", css: { 'text-align': 'center' } },
                           { id: "idMD", header: "idMD", hidden: true },
                           { id: "devName", header: ["Прибор", { content: "textFilter" }], width: 160, sort: "string" },
                          // { id: "param_value", header: ["ГВС", { content: "textFilter" }], width: 60 },
                           { id: "lastEnryJournal", header: ["Журнал", { content: "selectFilter" }], width: 190, sort: "string" },
                           {
                               id: "lastConnection", header: "Связь с ТВЧ",
                               template: function (obj) {
                                   //var date = new Date(parseInt(obj.date.replace(/\/Date\((\d+)\)\//, '$1')));
                                   var lastConnection = moment(obj.lastConnection).fromNow();
                                   return lastConnection;
                               },
                               width: 160, sort: "date", format: webix.i18n.dateFormatStr
                           },
                           /*{ id:"zabbix_time",	header:"На связи с ",
                                   template:function(obj){
                                   var date = new Date(parseInt(obj.zabbix_time.replace(/\/Date\((\d+)\)\//, '$1')));
                                   date = moment(date).fromNow();
                                   return date;
                               },
                           width:190, sort:"date", format:webix.i18n.dateFormatStr, hidden:true},
                           */
                           { id: "countTrub", header: ["Труб", { content: "selectFilter" }], width: 50, sort: "int", css: { 'text-align': 'center' } },
                          // { id: "system", header: "Система", width: 85, },
                         /*  { id: "teplo_source", header: ["Источник", { content: "selectFilter" }], width: 200, sort: "string", footer: { content: "countRows" } },*/
                           {
                               template: "<input type='button' class='webix_button webixtype_danger' value='Архив'></span>"
                           },
                           {
                               id: "last_day_arc", header: "Суточный", width: 160, sort: "date", template: function (obj) {
                                   var last_day_arc = moment(obj.last_day_arc).fromNow();
                                   return last_day_arc;
                               },


                           },
                           {
                          id: "last_hour_arc", header: "Часовой", width: 160, sort: "date", template: function (obj) {
                              var last_hour_arc = moment(obj.last_hour_arc).fromNow();
                              return last_hour_arc;
                                            },


                            },
                           /*{
                                  template: "<input type='button' class='webix_button webixtype_form' value='Опрос'></span>"
                              },
                              */

                       ],
                       fixedRowHeight: false, rowLineHeight: 35, rowHeight: 35, autowidth: true, resizeColumn: true,
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
                                   if (xmlHttp.readyState == 4 || xmlHttp.readyState == "complete") {

                                   }
                               });
                           },
                       },
                       sort: {
                           by: "#address#",
                           dir: "asc",
                           as: "string"
                       },
                       on: {
                           "onItemClick": function (id, e, trg, item) {
                               if (id.column == "lastEnryJournal") {
                                   //webix.message("Click on row: " + id.row+", column: " + id.column)
                                   //Open Windows New journalEntry
                                   //ShowWindowJournal();
                                   var item = this.getItem(id);
                                   ShowWindowJournal(item.idMD, item.address);
                                   }
                           },
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
                               webix.delay(function () {
                                   this.adjustRowHeight("lastEnryJournal", true);
                                   this.render();
                               }, this);
                           },
                           "onColumnResize": webix.once(function () {
                               this.adjustRowHeight("lastEnryJournal", true);
                           })

                       },
                       scheme: {
                           $change: function (item) {
                               if (item.idUkut === 'Завышение') {
                                   item.$css = "highlight";
                               }
                               if (item.idUkut === 'Нет связи') {
                                   item.$css = "errlight";
                               }
                               if (item.idUkut === 'Занижение') {
                                   item.$css = "lowlight";
                               }
                           }
                       },
                       //rowLineHeight: 15, rowHeight: 35,
                       autowidth: true,
                       footer: true
                       //autoheight:true
                   }
            ],

        }
    }).show();
    webix.i18n.setLocale("ru-RU");
    webix.extend($$("grid_ukuts"), webix.ProgressBar);
    $$("grid_ukuts").load("/ukut.asmx/GetListForJournals");
    $$("grid_ukuts").sort("#address#", "asc", "string");
}
function testAjax(url) {
    $.ajax({
        url: url,
        type: "GET",
        success: function (data) {
            //alert(data);
            return data;
        }
    });
}
function ShowWindowJournal(KPNum, address) {

    var form = {
        view: "form",
        id: "frm",
        borderless: true,
        elements: [
            {
                view: "combo", width: 300,
                label: 'Автор', name: "source",
                value: 1, yCount: "3", options: [
                    { id: "Пастухов А.В.", value: "Пастухов А.В." },
                    { id: "Ваганов М.С.", value: "Ваганов М.С." }
                ]
            },

            { view: "textarea", height: 200, label: "Текст сообщения", labelPosition: "top", name: "note" },
            {
                view: "button", value: "Отправить", click: function () {
                    var form_values = this.getParentView();
                    if (form_values.validate()) {
                        var url = '/ukut.asmx/InsertJournalEntry?kpnum=' + KPNum + '&text=' + form_values.getValues().note + '&author=' + form_values.getValues().source;
                        var newticket = testAjax(url);
                        sleep(1000);
                        $$('wtickets').getBody().getChildViews()[1].load("/ukut.asmx/ShowJournalEntrys?kpnum=" + KPNum);
                        $$('wtickets').getBody().getChildViews()[1].refresh();
                        $$('wtickets').getBody().getChildViews()[1].show();
                        this.getTopParentView().hide();
                        $$("grid_ukuts").clearAll();
                        $$("grid_ukuts").load("/ukut.asmx/GetListForJournals");

                    }
                    else {
                        webix.message({ type: "error", text: "Не все поля заполнены" });
                    }
                }
            },
            {
                view: "button", value: "Отмена", click: function () {
                    this.getTopParentView().hide();
                }
            }
        ],
        rules: {
            "source": webix.rules.isNotEmpty,
            "note": webix.rules.isNotEmpty
        },
        elementsConfig: {
            labelPosition: "top",
        }
    };


    webix.ui({
        view: "window",
        id: "new_ticket",
        width: 1100,
        position: "center",
        modal: true,
        head: {
            view: "toolbar", cols: [
                 { view: "label", label: "Новая запись" },
                 { view: "button", label: 'Закрыть', width: 100, align: 'right', click: function () { this.getTopParentView().hide(); } }
            ],
        },
        body: webix.copy(form),
        on: {
            "onShow": webix.once(function () {
                //this.getBody().getChildViews()[0].setValue("alex.vl.pas@outlook.com");
            })
        },
    });

    webix.ui({
        view: "window", id: "wtickets",
        head: {
            view: "toolbar", cols: [
                            { view: "label", label: "Журнал по адресу: <strong>" + address + "</strong>" },
                            { view: "button", label: 'Закрыть', width: 100, align: 'right', click: function () { this.getTopParentView().close(); } }
            ]
        },
        body: {
            rows: [
                   {
                       view: "button",
                       width: 300, value: 'Новая заявка',
                       click: function () { showForm("new_ticket") }
                   },
                   {
                       view: "datatable",
                       id: "grid_tickets2",
                       width: 300,
                       columns: [
                           { id: "ID", header: "#", width: 50 },
                           {
                               id: "dateStart", header: "Дата", template: function (obj) {
                                   var date = moment(obj.dateStart);
                                   return date.format("DD.MM.YYYY HH:mm");
                               }, width: 160
                           },
                           { id: "author", header: "Автор", width: 200 },
                           { id: "message", header: "Текст", width: 500 },
                          // { id: "status", header: "Статус", width: 160 },

                       ],
                       on: {
                           "onresize": webix.once(function () {
                               this.adjustRowHeight("message", true);
                           }),
                           "onLoadError": webix.once(function () {
                               this.hide();
                           }),

                       },
                       scheme: {
                           $change: function (item) {
                               if (item.closed === null)
                                   item.$css = "highlight";
                           }
                       },
                       tooltip: true,
                       fixedRowHeight: false, rowLineHeight: 25, rowHeight: 65,
                       autowidth: true,
                       autoheight: true
                   }
            ],

        }
    }).show();
    $$("grid_tickets2").load("/ukut.asmx/ShowJournalEntrys?kpnum=" + KPNum);
}
function showForm(winId, node) {
    $$(winId).getBody().clear();
    $$(winId).show(node);
    $$(winId).getBody().focus();
}
function sleep(milliseconds) {
    var start = new Date().getTime();
    for (var i = 0; i < 1e7; i++) {
        if ((new Date().getTime() - start) > milliseconds) {
            break;
        }
    }
}
function ShowWindowArchives(idMD, address, dateFrom, dateTo)
{
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