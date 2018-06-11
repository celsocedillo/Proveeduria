
var tabbodega;
var ldata = [];
var bodega;


function CargaDatos() {
    var $this = $(this);

    $.ajax({
        type: "POST",
        traditional: true,
        datatype: "json",
        data: null,
        contentType: 'application/json; charset=utf-8',
        url: "/Bodega/GetListaBodega",
        beforeSend: function () {
            run_waitMe($(".box-body"), 'win8', 'Cargando...');
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
                //var items = data.items;
                var dato = $.parseJSON(data.data);
                //tipomov = data[0]; //Esto es para guardar la estructura del objeto y poder usarlo en el momento de crear uno nuevo
                tabbodega.clear();
                tabbodega.rows.add(dato);
                tabbodega.draw();
            }
            $(".box-body").waitMe('hide');
            $this.button('reset');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $(".box-body").waitMe('hide');
            $this.button('reset');
            swal({
                type: 'error',
                text: 'Error al cargar los datos.',
                confirmButtonColor: '#00BCD4'
            });
        },
        complete: function () {
            calcularTotales();
        }
    });

}

function LimpiarFormularioBodega() {
    $("#lblIdBodega").text("");
    $("#txtNombre").val("");
    $("#txtCuentaContable").val("");
}



function Grabar() {
    $('#frmBodega').parsley().validate();
    if ($('#frmBodega').parsley().isValid()) {
        bodega = {
            "ID_BODEGA": $("#lblIdBodega").text(),
            "NOMBRE": $("#txtNombre").val(),
            "CUENTA_CONTABLE": $("#txtCuentaContable").val()
        };

        var parametros = JSON.stringify(
            {
                precord: bodega
            });
        $.ajax({
            type: "POST",
            traditional: true,
            datatype: "json",
            data: parametros,
            contentType: 'application/json; charset=utf-8',
            url: "/Bodega/Grabar",
            beforeSend: function () {
                run_waitMe($("#dlgBodega"), 'win8', 'Cargando...');
            },
            success: function (data) {
                if (data.error) {
                    swal({
                        type: 'error',
                        text: 'Error al grabar los datos.',
                        confirmButtonColor: '#00BCD4'
                    });
                }
                else {
                    $('#dlgBodega').modal('toggle');
                    swal({
                        type: 'success',
                        text: 'Datos grabados con éxito',
                        confirmButtonColor: '#00BCD4'
                    });
                    CargaDatos();
                }
                $("#dlgBodega").waitMe('hide');
                //$this.button('reset');
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $("#dlgBodega").waitMe('hide');
                //$this.button('reset');
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

//$(document).ready(function () {
//    tabbodega = $('#tabBodega').DataTable({

var Bodega = function () {
    return {
        init: function () {
            tabbodega = $('#tabBodega').DataTable({
                "paging": true,
                "searching": false,
                //"filter": false,
                //"dom": 't',
                "autoWidth": false,
                "columnDefs":
                    [
                        { "targets": [0],/* "width": "10%",*/ "visible": true, "orderable": false },
                        { "targets": [3],/* "width": "10%",*/ "defaultContent": '<button id="butEditar" type="button" class="btn btn-default btn-xs clsedit"><i class="fa fa-pencil"></i></button>' }
                    ],
                "data": ldata,
                //"rowCallback": function (row, data, dataIndex) {
                //},
                "columns": [
                    { "data": 'ID_BODEGA' },
                    { "data": 'NOMBRE' },
                    { "data": 'CUENTA_CONTABLE' }
                ]
            });

            CargaDatos();

            tabbodega.on('click', 'button.clsedit', function (e) {
                var row = $(this).closest('tr');
                var data = tabbodega.row($(this).parents('tr')).data();
                var parametros = JSON.stringify(
                    {
                        pid: data.ID_BODEGA
                    });

                $.ajax({
                    dataType: 'JSON',
                    //url: '@Url.Action("GetBodega", "Bodega")',
                    url: '/Bodega/GetBodega',
                    type: "POST",
                    contentType: 'application/json; charset=utf-8',
                    data: parametros,
                    success: function (result) {
                        if (result.resultado == "success") {
                            $("#dlgBodega").modal('toggle');
                            $("#lblIdBodega").text(result.ID_BODEGA);
                            $("#txtNombre").val(result.NOMBRE);
                            $("#txtCuentaContable").val(result.CUENTA_CONTABLE);
                        }
                        else {
                            swal({
                                type: 'error',
                                text: 'Error al grabar los datos.',
                                confirmButtonColor: '#00BCD4'
                            });
                        }
                    },
                    error: function (result) {
                        alert("Error :" + result.responseText);
                        $("#msgMain").html(result.responseText);
                        $("#msgMain").addClass("alert alert-danger");
                    }
                });
            });

            $("#butAcepta").on("click", function () {
                Grabar();
            });

            $("#butNuevoBodega").on("click", function () {
                bodega = { ID_BODEGA: 0, NOMBRE: "", CUENTA_CONTABLE: "", ESTADO: "A" };
                LimpiarFormularioBodega();
                $("#dlgBodega").modal('toggle');
            });
        }
    };
}();
