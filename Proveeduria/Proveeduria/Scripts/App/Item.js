var tabStock;
var item_bodega;

function CargaStock() {
    var $this = $(this);
    var parametros = JSON.stringify(
        {
            pid: $("#ID_ITEM").val()
        });
    $.ajax({
        type: "POST",
        traditional: true,
        datatype: "json",
        data: parametros,
        contentType: 'application/json; charset=utf-8',
        url: "/Item/GetStockBodega",
        beforeSend: function () {
            run_waitMe($(".box-body"), 'Cargando...');
        },
        success: function (data) {
            if (data.error) {
                swal({
                    type: 'error',
                    text: 'Error al cargar los datos.',
                    confirmButtonColor: '#00BCD4'
                });
            }
            else {
                item_bodega = (data.data);
                tabStock.clear();
                tabStock.rows.add(data);
                tabStock.draw();
            }
            $(".box-body").waitMe('hide');
            $this.button('reset');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $(".box-body").waitMe('hide');
            swal({
                type: 'error',
                text: 'Error al cargar los datos.',
                confirmButtonColor: '#00BCD4'
            });
        },
        complete: function () {
            
        }
    });

}

function AgregarBodega(registro) {
    var cantidad_minima = `<div class="form-group" style="text-align: rigth;display:inline-block;">
                                    <input id=txtcantminima name="txtcantminima" type="number" value="${registro[0].CANTIDAD_MINIMA}" class="form-control text-right txtcant no-spin" min=0 step="any" required>
                                   </div>`;
    var cantidad_critica = `<div class="form-group" style="text-align: rigth;display:inline-block;">
                                    <input id=txtcantcritica name="txtcantcritica" type="number" value="${registro[0].CANTIDAD_CRITICA}" class="form-control text-right txtcant no-spin" min=0 step="any" required>
                                   </div>`;
    var cantidad_maxima = `<div class="form-group" style="text-align: rigth;display:inline-block;">
                                    <input id=txtcantmaxima name="txtcantmaxima" type="number" value="${registro[0].CANTIDAD_MAXIMA}" class="form-control text-right txtcant no-spin" min=0 step="any" required>
                                   </div>`;
    var row = []
    row.push({
        "ID_ARTIBODE": registro[0].ID_ARTIBODE,
        "ID_BODEGA": registro[0].ID_BODEGA,
        "BODEGA_NOMBRE": registro[0].BODEGA_NOMBRE,
        "CANTIDAD_ACTUAL": registro[0].CANTIDAD_ACTUAL,
        "CANTIDAD_CRITICA": cantidad_critica,
        "CANTIDAD_MAXIMA": cantidad_maxima,
        "CANTIDAD_MINIMA": cantidad_minima,
        "CANTIDAD_INICIO": registro[0].CANTIDAD_INICIO
    });
    tabStock.rows.add(row).draw();
    tabStock.columns.adjust().draw();
}

function Grabar() {
    $('#frmItem').parsley().validate();
    if ($('#frmItem').parsley().isValid()) {
        var forma = serializaForma($("#frmItem"));
        forma["EPRTA_ARTICULO_BODEGA"] = [];
        if (tabStock.rows().count() > 0) {
            $("#tabStock tbody tr").each(function () {
                var fila = $(this).closest('tr');
                linea_data = tabStock.row(tabStock.row(fila).index());
                forma["EPRTA_ARTICULO_BODEGA"].push({
                    "ID_ARTIBODE": linea_data.data()["ID_ARTIBODE"],
                    "ID_BODEGA": linea_data.data()["ID_BODEGA"],
                    "CANTIDAD_MAXIMA": $('input[name="txtcantmaxima"]', fila).val(),
                    "CANTIDAD_MINIMA": $('input[name="txtcantminima"]', fila).val(),
                    "CANTIDAD_CRITICA": $('input[name="txtcantcritica"]', fila).val(),

                });
            });
         }

        var parametros = (
            {
                precord: forma
            }); 
        $.ajax({
            type: "POST",
            traditional: true,
            datatype: "json",
            data: JSON.stringify(forma),
            //data: parametros,
            contentType: 'application/json; charset=utf-8',
            url: "/Item/Grabar",
            beforeSend: function () {
                run_waitMe($(".box-primary"), 'Grabando...');
            },
            success: function (data) {
                if (data.resultado == "success") {
                    swal({
                        type: 'success',
                        text: 'Datos grabados',
                        timer: 20000,
                        confirmButtonColor: '#00BCD4'
                    }).then(function () {
                        window.location.href = '/Item/ListaItem';
                    });
                    
                }
                else {
                    swal({
                        type: data.resultado,
                        text: 'Error al grabar los datos. ' + data.mensaje,
                        confirmButtonColor: '#00BCD4'
                    });
                }
                $(".box-primary").waitMe('hide');
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $(".box-primary").waitMe('hide');
                swal({
                    type: 'error',
                    text: 'Error al cargar los datos.',
                    confirmButtonColor: '#00BCD4'
                });
            },
            complete: function () {
            }
        });
    }

}

var Item = function () {
return {
    init: function () {

        //$("#FECHA_ULTIMO").datepicker({
        //    format: "dd/mm/yyyy",
        //    todayHighlight: true,
        //    autoclose: true
        //});


        $("#btnCancelar").on('click', function () {
            window.location.href = '/Item/ListaItem' ;
        });

        tabStock = $('#tabStock').DataTable({
            "title" : "Items por Bodega",
            "dom": 't',
            "autoWidth": false,
            //"rowCallback": function (nRow, aData, iDisplayIndex) {
            //    if (aData != null && aData != "") {
            //        $('td', nRow).eq(2).css({ background: "#34485e" });
            //    }
            //},
            "columnDefs":
                    [
                    { "targets": [2, 4, 5, 6, 7], "orderable": false, "className": "text-right" },
                    { "targets": [3], "orderable": false, "className": "columna_stock" }
                ],
            "columns": [
                { "data": 'ID_ARTIBODE' },
                { "data": 'ID_BODEGA' },
                { "data": 'BODEGA_NOMBRE' },
                { "data": 'CANTIDAD_ACTUAL' },
                { "data": 'CANTIDAD_CRITICA' },
                { "data": 'CANTIDAD_MAXIMA' },
                { "data": 'CANTIDAD_MINIMA' },
                { "data": 'CANTIDAD_INICIO' }
            ]
        });

        tabStock.clear();
        if (lstock_bodega.length > 0) {
            AgregarBodega(lstock_bodega);
        } else {
            var xregbod =[
            {
                ID_ARTIBODE: 0,
                ID_BODEGA: SId_bodega,
                BODEGA_NOMBRE: SBodega,
                CANTIDAD_ACTUAL: 0,
                CANTIDAD_CRITICA: 0,
                CANTIDAD_MAXIMA: 0,
                CANTIDAD_MINIMA: 0,
                CANTIDAD_INICIO: 0
            }]
            AgregarBodega(xregbod);
        }
        
        column = tabStock.column(0);
        column.visible(false);
        column = tabStock.column(1);
        column.visible(false);


        //CargaStock();

        $("#btnGrabar").on('click', function () {
            Grabar();
        });

        if ($("#ESTADO").val() == "N") {

        } else {
            $("#CODIGO").prop('readonly', true);
        }

        
    }
};
}();