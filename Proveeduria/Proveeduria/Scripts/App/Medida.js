var tabMedida;
var lmedida = [];
var medida;


function CargaDatosMedida() {
    var $this = $(this);

    $.ajax({
        type: "POST",
        traditional: true,
        datatype: "json",
        data: null,
        contentType: 'application/json; charset=utf-8',
        url: "/Configuracion/GetListaMedida",
        beforeSend: function () {
            run_waitMe($(".box-body"), 'win8', 'Cargando...');
        },
        success: function (data) {
            if (data.resultado == "error") {
                swal({
                    type: 'error',
                    text: 'Error al cargar los datos.'+' Aplicacion Msg: '+ data.msg,
                    confirmButtonColor: '#00BCD4'
                });
            }
            else if (data.resultado == "success"){
                lmedida = (data.data);
                tabMedida.clear();
                tabMedida.rows.add(lmedida);
                tabMedida.draw();
            }
            $(".box-body").waitMe('hide');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $(".box-body").waitMe('hide');
            $this.button('reset');
            swal({
                type: 'error',
                text: 'Error al cargar los datos.',
                confirmButtonColor: '#00BCD4'
            });
        }
    });

}

function LimpiarFormularioMedida() {
    $("#lblIdMedida").text("");
    $("#txtNombreMedida").val("");
    $("#sltEstadoMedida").val("A").trigger('change.select2');
}



function GrabarMedida() {
    $('#frmMedida').parsley().validate();
    if ($('#frmMedida').parsley().isValid()) {
        medida = {
            "ID_MEDIDA": $("#lblIdMedida").text(),
            "NOMBRE": $("#txtNombreMedida").val(),
            "ESTADO": $("#sltEstadoMedida").val()
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
            url: "/Configuracion/GrabarMedida",
            beforeSend: function () {
                run_waitMe($("#dlgMedida"), 'win8', 'Cargando...');
            },
            success: function (data) {
                if (data.resultado == "error") {
                    swal({
                        type: 'error',
                        text: 'Error al grabar los datos. Aplicacion Msg: ' + data.msg,
                        confirmButtonColor: '#00BCD4'
                    });
                }
                else if (data.resultado == "success"){
                    $('#dlgMedida').modal('toggle');
                    swal({
                        type: 'success',
                        text: 'Datos grabados con éxito',
                        confirmButtonColor: '#00BCD4'
                    });
                    CargaDatosMedida();
                }
                $("#dlgMedida").waitMe('hide');
                //$this.button('reset');
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $("#dlgMedida").waitMe('hide');
                swal({
                    type: 'error',
                    text: 'Error en la llamada al servidor.',
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
            tabMedida = $('#tabMedida').DataTable({
                "dom": 't',
                "autoWidth": false,
                "columnDefs":
                    [
                        { "targets": [0], "visible": false, "orderable": false },
                        {
                            "targets": [2],
                            render: function (data, type, row) {
                                var color = 'black';
                                if (data == 'Inactivo') {
                                    color = 'red';
                                }
                                return '<span style="color:' + color + '">' + data + '</span>'
                            }
                        },
                        { "targets": [3], "defaultContent": '<a href=# id="butEditar" class="clsedit" ><i class="fa fa-pencil"></i></a>', "className" : "text-center"  }
                    ],
                "data": lmedida,
                "columns": [
                    { "data": 'ID_MEDIDA' },
                    { "data": 'NOMBRE' },
                    { "data": 'ESTADO_REGISTRO' }
                ]
            });

            CargaDatosMedida();

            tabMedida.on('click', 'a.clsedit', function (e) {
                var row = $(this).closest('tr');
                var data = tabMedida.row($(this).parents('tr')).data();
                var parametros = JSON.stringify(
                    {
                        pid: data.ID_MEDIDA
                    });

                $.ajax({
                    dataType: 'JSON',
                    url: '/Configuracion/GetMedida',
                    type: "POST",
                    contentType: 'application/json; charset=utf-8',
                    data: parametros,
                    success: function (result) {
                        if (result.resultado == "success") {
                            medida = result.data;
                            $("#dlgMedida").modal('toggle');
                            $("#lblIdMedida").text(medida.ID_MEDIDA);
                            $("#txtNombreMedida").val(medida.NOMBRE);
                            $("#sltEstadoMedida").val(medida.ESTADO).trigger('change.select2');
                        }
                        else if (result.resultado == "error") {
                            swal({
                                type: 'error',
                                text: 'Error al cargar los datos. Aplicacion Msg:' + result.msg,
                                confirmButtonColor: '#00BCD4'
                            });
                        }
                    },
                    error: function (result) {
                        swal({
                            type: 'error',
                            text: 'Error en la llamada al servidor',
                            confirmButtonColor: '#00BCD4'
                        });
                        $("#msgMain").html(result.responseText);
                        $("#msgMain").addClass("alert alert-danger");
                    }
                });
            });

            $("#butAceptaMedida").on("click", function () {
                GrabarMedida();
            });

            $("#butNuevoMedida").on("click", function () {
                medida = { ID_MEDIDA: 0, NOMBRE: "" };
                LimpiarFormularioMedida();
                $("#dlgMedida").modal('toggle');
            });
        }
    };
            //});
}();

