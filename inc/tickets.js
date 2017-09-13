function ShowWindowTickets(idUkut) {
    var form = {
        view: "form",
        id: "frm",
        borderless: true,
        elements: [
            {
                view: "combo", width: 300,
                label: 'Автор', name: "source",
                value: 1, yCount: "3", options: [
					{ id: "h.rafikova@ougk.ru", value: "ЖЭУ 1" },
					{ id: "n.tokareva@ougk.ru", value: "ЖЭУ 2" },
					{ id: "d.lahtin@ougk.ru", value: "ЖЭУ 3" },
					{ id: "g.yablonskih@ougk.ru", value: "ЖЭУ 4" },
					{ id: "t.bondareva@ougk.ru", value: "ЖЭУ 5" },
					{ id: "e.burlakova@ougk.ru", value: "ЖЭУ 6" },
					{ id: "n.sannikova@ougk.ru", value: "ЖЭУ 7" },
					{ id: "o.necelya@ougk.ru", value: "ЖЭУ 8" },
					{ id: "n.neustroeva@ougk.ru", value: "ЖЭУ 9" },
					{ id: "n.orehova@ougk.ru", value: "ЖЭУ 10" },
					{ id: "groshev@ougk.ru", value: "Грошев О.Г." },
					{ id: "a.ozornin@ougk.ru", value: "Озорнин А.Ф." },
					{ id: "a.fegler@ougk.ru", value: "Феглер А.Г." },
					{ id: "v.sanin@ougk.ru", value: "Санин В.В." },
					{ id: "a.krasnov@ougk.ru", value: "Краснов А." },
					{ id: "rec@ougk.ru", value: "Подлеснов Ю.В." },
					{ id: "alex.vl.pas@gmail.com", value: "Пастухов А.В." }
                ]
            },
            { view: "textarea", height: 200, label: "Текст заявки", labelPosition: "top", name: "note" },
            {
                view: "button", value: "Отправить", click: function () {
                    var form_values = this.getParentView().getValues();
                    var url = 'ukut.asmx/CreateTicket?kpnum=' + pmid + '&message=' + form_values.note + '&author=' + form_values.source + '&address=' + aaddress + '&email=' + form_values.source;
                    var newticket = testAjax(url);
                    sleep(1000);
                    $$('wtickets').getBody().getChildViews()[1].load("ukut.asmx/ShowTickets?kpnum=" + pmid);
                    $$('wtickets').getBody().getChildViews()[1].refresh();
                    $$('wtickets').getBody().getChildViews()[1].show();
                    this.getTopParentView().hide();
                }
            },
            {
                view: "button", value: "Отмена", click: function () {
                    this.getTopParentView().hide();
                }
            }
        ],
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
                 { view: "label", label: "Новая заявка" },
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
                            { view: "label", label: "Заявки по адресу: <strong>" + aaddress + "</strong>" },
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
                               id: "dateStart", header: "Открыта", template: function (obj) {
                                   var date = moment(obj.dateStart);
                                   return date.format("DD.MM.YYYY HH:mm");
                               }, width: 160
                           },
                           { id: "author", header: "Автор", width: 200 },
                           { id: "message", header: "Текст", width: 500 },
                           { id: "status", header: "Статус", width: 160 },

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
                           //$group:"ticket_id",
                           //$sort:{ by:"created", dir:"desc" },
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
    $$("grid_tickets2").load("ukut.asmx/ShowTickets?kpnum=" + idUkut);
    //$$("grid_tickets2").adjustRowHeight("body");
}