function ShowWindowUkuts() {
    webix.ui.datafilter.countRows = webix.extend({
        refresh: function (master, node, value) {
            node.firstChild.innerHTML = master.count();
        }
    }, webix.ui.datafilter.summColumn);
    webix.protoUI({
        name: "activeTable"
    }, webix.ui.datatable, webix.ActiveContent);
    webix.ui({
        view: "window", id: "utickets", fullscreen:true,
        head: {
            view: "toolbar", cols: [
                            { view: "label", label: "Редактор объектов на карте" },

                            {
                                view: "button", label: 'Excel', width: 100, align: 'right', click: function () {
                                    webix.toExcel($$("grid_ukuts"));
                                }
                            },
                                //{ view: "button", label: 'Закрыть', width: 100, align: 'right', click: function () { this.getTopParentView().close(); } },
            ]
        },
        body: {
            rows: [
                {
                    view:"toolbar", elements:[
                        { view:"button", value:"Добавить узел", click:function(){
                            $$('grid_ukuts').add({
                                
                            },0);
                        }},
                        { view:"button", value:"Удалить узел", click:function(){
                            var id = $$('grid_ukuts').getSelectedId();
                            if (id)
                                $$('grid_ukuts').remove(id);
                        }},
                                {
                                    view: "button", value: "Загрузить координаты", click: function () {
                                        var id = $$('grid_ukuts').getSelectedId();
                                        var record = $$('grid_ukuts').getItem(id);
                                        OpenYandexWindow(record.address);
                                    }
                                },
                    ]
                },
                   {
                       view: "activeTable",
                       id: "grid_ukuts",
                       leftSplit: 2,
                       columns: [
                          
                           { id: "idUKUT", adjust: "all", header: "#", width: 50},
                           { id: "address", adjust: "all", header: ["Адрес", { content: "textFilter" }], width: 200, sort: "string", footer: { text: "Всего:", colspan: 3 }, editor: "text" },
                           { id: "geu", adjust: "all", header: ["ЖЭУ", { content: "textFilter" }], editor: "text" },
                           { id: "Abonent", adjust: "all", header: ["Абонент",{ content: "textFilter" }], editor: "text" },
                           { id: "Dogovor", adjust: "all", header: "Договор", editor: "text" },
                           { id: "dQotop", adjust: "all", header: "Qотоп", editor: "text" },
                           { id: "dGgvs", adjust: "all", header: "Qгвс", editor: "text" },
                           { id: "tcw", adjust: "all", header: "tхи", editor: "text" },
                           { id: "idMD", adjust: "all", header: "Соотвествие в базе SCADA", editor: "select", options: "/ukut.asmx/GetKPTable", },
                           { id: "devName", adjust: "all", header: "Прибор", editor: "text" },
                           { id: "SerialNum", adjust: "all", header: ["Серийный номер",{ content: "textFilter" }], editor: "text" },
                           {
                               id: "devType", adjust: "all", header: ["Тип прибора",{ content: "textFilter" }], editor: "select",
                               options: [
                                   { id: 1, value: "СПТ943" },
                                   { id: 2, value: "СПТ942" },
                                   { id: 3, value: "СПТ941" },
                                   { id: 4, value: "СПТ943.10" },
                                   { id: 11, value: "Карат 2001" },
                                   { id: 51, value: "Тэкон-17" },
                                   { id: 58, value: "ТС-07" },
                                   { id: 169, value: "MB-110.8A" },
                               ],
                           },
                           {
                               id: "lon", adjust: "all", header: "lon", editor: "text", 
                           },
                           { id: "lat", adjust: "all", header: "lat", editor: "text", },
                           {
                               id: "teplo_source", adjust: "all", header: ["Источник", { content: "selectFilter" }],
                               width: 100, sort: "string", editor: "text", 
                               //collection: "../config/teplo_source.json",
                           },
    
                           {
                                id: "column_pod", adjust: "all", header: ["Подача", { content: "textFilter" }],
                                 width: 160, sort: "string", editor: "select", options: [" ", "TB1_t1", "TB2_t1"], 
                           },
                           {
                                id: "column_obr", adjust: "all", header: ["Обратка", { content: "textFilter" }],
                                width: 160, sort: "string", editor: "select", options: [" ", "TB1_t2", "TB2_t2"], 
                           },
                           {
                                 id: "column_gvs", adjust: "all", header: ["ГВС", { content: "textFilter" }],
                                 width: 160, sort: "string", editor: "select", options: [" ", "TB1_t1", "TB2_t1", "TB1_t3"],
                            },
                         /*  { id: "highway", adjust: "all", header: "Магистраль", editor: "text" },  
                           { id: "teplo_camera", adjust: "all", header: "Камера", editor: "text" },
                           */
                           {
                               id: "countTrub", adjust: "all", header: ["Труб", { content: "selectFilter" }],
                               width: 50, sort: "int", css: { 'text-align': 'center' }, editor: "text",
                           },
                           {
                               id: "system", adjust: "all", header: "Система",
                               width: 85, editor: "select", options: [{ id: 1, value: "Закрытая" }, { id: 2, value: "Открытая" }],
                           },
                           {
                               id: "primechanie", adjust: "all", header: ["Примечание", { content: "selectFilter" }],
                               width: 50, sort: "int", css: { 'text-align': 'center' }, editor: "text"
                           },
                           {
                                  id: "formula_winter", adjust: "all", header: ["Зимняя", { content: "selectFilter" }],
                                  width: 50, sort: "int", css: { 'text-align': 'center' }, editor: "text"
                           },
                           {
                                  id: "formula_summer", adjust: "all", header: ["Летняя", { content: "selectFilter" }],
                                  width: 50, sort: "int", css: { 'text-align': 'center' }, editor: "text"
                           },
                              {
                                  id: "scheme", adjust: "all", header: ["Схема", { content: "selectFilter" }],
                                  width: 50, sort: "int", css: { 'text-align': 'center' }, editor: "text"
                              },
                                 {
                                     id: "uchet", adjust: "all", header: ["Учет", { content: "selectFilter" }],
                                     width: 50, sort: "int", css: { 'text-align': 'center' }, editor: "text"
                                 },

        
                         
                       ],
                       editable: true,
                       url: "/ukut.asmx/GetUkuts?list=yes",
                       save: "/ukut.asmx/GetUkuts",
                       on:{
                           "onAfterEditStop": webix.once(function (state, editor, ignoreUpdate) {
                               webix.message("Изменено");
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
                       sort: {
                           by: "#address#",
                           dir: "asc",
                           as: "string"
                       },

                       // rowLineHeight: 15, rowHeight: 35,
                       //autowidth: true,
                       footer: true,
                       select:true,
                       //autoheight:true
                   },

            ],

        }
    }).show();
    webix.extend($$("grid_ukuts"), webix.ProgressBar);    
}
function OpenYandexWindow(address)
{
    var myGeocoder = ymaps.geocode("г.Екатеринбург, "+address);
    myGeocoder.then(
        function (res) {
            var coords = res.geoObjects.get(0).geometry.getCoordinates();
            var id = $$('grid_ukuts').getSelectedId();
            var record = $$('grid_ukuts').getItem(id);
            record.lon = coords[0].toFixed(6);
            record.lat = coords[1].toFixed(6);
            $$('grid_ukuts').updateItem(id, record);
        },
        function (err) {
            alert('Ошибка, попробуйте ввести координаты вручную!');
        }
    );
}