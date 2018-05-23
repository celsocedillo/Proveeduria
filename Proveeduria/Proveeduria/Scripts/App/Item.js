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

function Grabar() {
    $('#frmItem').parsley().validate();
    if ($('#frmItem').parsley().isValid()) {
        var forma = serializaForma($("#frmItem"));
        $.ajax({
            type: "POST",
            traditional: true,
            datatype: "json",
            data: JSON.stringify(forma),
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
            //"bSortable": false
            "columnDefs":
                    [
                    { "targets": [0,1,2,3,4,5], "orderable": false, "className" : "text-right" }
                ]
        });

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