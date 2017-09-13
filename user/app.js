
function GetTable() {
    dateFrom = moment().startOf('month').subtract(1, 'month').format('01.MM.YYYY');
    dateTo = moment().subtract(1, 'month').format('31.MM.YYYY');
    webix.i18n.setLocale("ru-RU");
    //Текущие
    webix.ui({
        view: "window", id: "wizmarchives",  move: true, resize: true, position:function(state){ 
            state.left = 20; //fixed values
            state.top = 20;
        },
        head: {
            view: "toolbar", cols: [
                            { view: "label", label: "Мгновенные по системе теплоснабжения", align: "center"},
            ]
        },
        body: {
            rows: [
                   {
                       view: "datatable",
                       id: "gizmarchives", width: 600, height: 280, select: "row", scrollX: true,

                       columns: [
                           {
                               id: "channel_name", header: ["Параметр"], fillspace: true,
                               width: 160
                           },

                           {
                               id: "channel_value", header: ["Значение"], width: 80, fillspace: true
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


                   }
            ],

        }
    }).show();
    webix.extend($$("gizmarchives"), webix.ProgressBar);

    //NEW
    webix.ui({
        view: "window", move: true, resize: true, position: function (state) {
            state.left = 700; //fixed values
            state.top = 20;
        },
        head: {
            view: "toolbar", cols: [
                            { view: "label", label: "Накопленные по системе теплоснабжения", align: "center" },
                   
            ]
        },
        body: {
            rows: [
                         {
                             view: "datepicker", id: "dateOT", width: 200, align: "left", bottomLabel: "* выберите месяц", type: "month", value: moment().startOf('month').subtract(1, 'month').format('YYYY.MM.DD'),
                         },
                   {
                       view: "datatable",
                       id: "marchives", width: 600, height: 150, select: "row", scrollX: true,

                       columns: [
                           {
                               id: "Date", header: ["Дата"], fillspace: true,
                               width: 180,			template:function(obj){
                                   //var date = new Date(parseInt(obj.date.replace(/\/Date\((\d+)\)\//, '$1')));
                                   var date = moment(obj.Date);
                                   return date.format("DD.MM.YYYY HH:mm");
                               },
                           },

                           {
                               id: "TB1_M1", header: ["Масса",  { text: "Подача", colspan: 3 }], width: 80, fillspace: true,
                               template: function (obj) {
                                   return round(obj.TB1_M1, 2);
                               }
                           },
                           {
                               id: "TB1_P1", header: "Давление", width: 80, fillspace: true,
                               template: function (obj) {
                                   return round(obj.TB1_P1, 2);
                               }
                           },
                           {
                               id: "TB1_t1", header: "Темп.", width: 80, fillspace: true,
                               template: function (obj) {
                                   return round(obj.TB1_t1, 2);
                               }

                           },
                           {
                               id: "TB1_M2", header: ["Масса", { text: "Обратка", colspan: 3 }], width: 80, fillspace: true,
                               template: function (obj) {
                                   return round(obj.TB1_M2, 2);
                               }
                           },
                           {
                               id: "TB1_P2", header: "Давление", width: 80, fillspace: true,
                               template: function (obj) {
                                   return round(obj.TB1_P2, 2);
                               }
                           },
                           {
                               id: "TB1_t2", header: "Темп.", width: 80, fillspace: true,
                               template: function (obj) {
                                   return round(obj.TB1_t2, 2);
                               }
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


                   }
            ],

        }
    }).show();
    webix.extend($$("marchives"), webix.ProgressBar);
    $$("dateOT").attachEvent("onChange", function (newv, oldv) {

        //webix.message("Value changed from: " + oldv + " to: " + newv);
        var dt = moment(newv).format('YYYY-MM-01 00:00');
        $$("marchives").clearAll();
        $$("marchives").load('/ukut.asmx/GetArchive?arc=month&idMD=35&dateFirst="' + dt + '"&dateLast="' + dt + '"');
    });
    ///ХВС
    


    webix.ui({
        view: "window", id: "hvs", move: true, position:function(state){ 
            state.left = 700; //fixed values
            state.top = 300;
        },
        head: {
            view: "toolbar", cols: [
                            { view: "label", label: "ХВС: ул. Бакинских Комиссаров 173 ", align: "center" },
                           
            ]
        },
        body: {
            rows: [
                {
                    cols: [
                         { view: "datepicker", id: "date", width: 200, align: "left", bottomLabel: "* выберите месяц", type: "month", value: moment().startOf('month').subtract(1, 'month').format('YYYY.MM.DD'), },
                       //{ view: "button", value: "Получить", width: 150, click: "GetTableHVS" },
                    ],
                },
                  {
                       view: "datatable",
                       id: "hvsarchive", width: 600, height: 100, select: "row", scrollX: true,

                       columns: [
                           {
                               id: "P", header: ["Показания"],  fillspace: true, 
                               
                           },
                           {
                               id: "M", header: ["Объем M3"], fillspace: true, 
                               template: function (obj) {
                                   return Math.round(obj.M);
                               },
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


                   }
            ],

        }
    }).show();
    webix.extend($$("hvs"), webix.ProgressBar);

    webix.i18n.setLocale("ru-RU");
    $$("gizmarchives").load('/ukut.asmx/GetCurrentValuesBak173?KPNum=35');
    $$("marchives").load('/ukut.asmx/GetArchive?arc=month&idMD=35&dateFirst="' + moment().startOf('month').subtract(1, 'month').format('YYYY-MM-01') + '"&dateLast="' + moment().startOf('month').subtract(1, 'month').format('YYYY-MM-01') + '"');
    $$("hvsarchive").load('/ukut.asmx/HVS?dateFrom=' + dateFrom + '&dateTo=' + dateTo + '');
    $$("date").attachEvent("onChange", function (newv, oldv) {

        var dateFrom = moment(newv).format('01.MM.YYYY');
        var dateTo = moment(newv).format('31.MM.YYYY');
        $$("hvsarchive").clearAll();
        $$("hvsarchive").load('/ukut.asmx/HVS?dateFrom=' + dateFrom + '&dateTo=' + dateTo + '');
    });
}
function round(a, b) {
    b = b || 0;
    return Math.round(a * Math.pow(10, b)) / Math.pow(10, b);
}