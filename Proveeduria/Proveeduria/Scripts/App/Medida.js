var tabmedida;
var ldata = [];
var medida;


function CargaDatos() {
    var $this = $(this);

    $.ajax({
        type: "POST",
        traditional: true,
        datatype: "json",
        data: null,
        contentType: 'application/json; charset=utf-8',
        url: "/Medida/GetListaMedida",
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
                var data = $.parseJSON(data.data);
                //tipomov = data[0]; //Esto es para guardar la estructura del objeto y poder usarlo en el momento de crear uno nuevo
                tabmedida.clear();
                tabmedida.rows.add(data);
                tabmedida.draw();
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

function LimpiarFormularioMedida() {
    $("#lblIdMedida").text("");
    $("#txtNombre").val("");
}



function Grabar() {
    $('#frmMedida').parsley().validate();
    if ($('#frmMedida').parsley().isValid()) {
        medida = {
            "ID_MEDIDA": $("#lblIdMedida").text(),
            "NOMBRE": $("#txtNombre").val()
        };

        var parametros = JSON.stringify(
            {
                precord: medida
            });
        $.ajax({
            type: "POST",
            traditional: true,
            datatype: "json",
            data: parametros,
            contentType: 'application/json; charset=utf-8',
            url: "/Medida/Grabar",
            beforeSend: function () {
                run_waitMe($("#dlgMedida"), 'win8', 'Cargando...');
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
                    $('#dlgMedida').modal('toggle');
                    swal({
                        type: 'success',
                        text: 'Datos grabados con éxito',
                        confirmButtonColor: '#00BCD4'
                    });
                    CargaDatos();
                }
                $("#dlgMedida").waitMe('hide');
                //$this.button('reset');
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $("#dlgMedida").waitMe('hide');
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
//    tabmedida = $('#tabMedida').DataTable({

var Medida = function () {
    return {
        init: function () {
            tabmedida = $('#tabMedida').DataTable({
                        "paging": true,
                        "searching": false,
                        //"filter": false,
                        //"dom": 't',
                        "autoWidth": false,
                        "columnDefs":
                            [
                                { "targets": [0],/* "width": "10%",*/ "visible": true, "orderable": false },
                                { "targets": [2],/* "width": "10%",*/ "defaultContent": '<button id="butEditar" type="button" class="btn btn-default btn-xs clsedit"><i class="fa fa-pencil"></i></button>' }
                            ],
                        "data": ldata,
                        //"rowCallback": function (row, data, dataIndex) {
                        //},
                        "columns": [
                            { "data": 'ID_MEDIDA' },
                            { "data": 'NOMBRE' }
                        ]
            });

            CargaDatos();

            tabmedida.on('click', 'button.clsedit', function (e) {
                var row = $(this).closest('tr');
                var data = tabmedida.row($(this).parents('tr')).data();
                var parametros = JSON.stringify(
                    {
                        pid: data.ID_MEDIDA
                    });

                $.ajax({
                    dataType: 'JSON',
                    //url: '@Url.Action("GetMedida", "Medida")',
                    url: '/Medida/GetMedida',
                    type: "POST",
                    contentType: 'application/json; charset=utf-8',
                    data: parametros,
                    success: function (result) {
                        if (result.resultado == "success") {
                            $("#dlgMedida").modal('toggle');
                            $("#lblIdMedida").text(result.ID_MEDIDA);
                            $("#txtNombre").val(result.NOMBRE);
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

            $("#butNuevoMedida").on("click", function () {
                medida = { ID_MEDIDA: 0, NOMBRE: "" };
                //if (medida == null) {
                //    medida = { ID_MEDIDA: 0, NOMBRE: "", ESTADO: "N" };
                //} else {
                //    medida.ID_MEDIDA = 0;
                //    medida.NOMBRE = "";
                //}
                LimpiarFormularioMedida();
                $("#dlgMedida").modal('toggle');
            });

//});
        }
    };
}();