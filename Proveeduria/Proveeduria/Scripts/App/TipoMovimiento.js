var tabTipoMov;
var ltipo = [];
var tipomov;


function CargaDatosTipo() {
    var $this = $(this);

    $.ajax({
        type: "POST",
        traditional: true,
        datatype: "json",
        data: null,
        contentType: 'application/json; charset=utf-8',
        url: "/Configuracion/GetListaTipoMovimiento",
        beforeSend: function () {
            run_waitMe($(".box-body"), 'Cargando...');
        },
        success: function (data) {
            if (data.resultado == "error") {
                swal({
                    type: 'error',
                    text: 'Error al cargar los datos. Aplicacion Msg:' + data.msg,
                    confirmButtonColor: '#00BCD4'
                });
            }
            else {
                var data = (data.data);
                tabTipoMov.clear();
                tabTipoMov.rows.add(data);
                tabTipoMov.draw();
            }
            $(".box-body").waitMe('hide');
            //$this.button('reset');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $(".box-body").waitMe('hide');
            $this.button('reset');
            swal({
                type: 'error',
                text: 'Error en la llamada al servidor ',
                confirmButtonColor: '#00BCD4'
            });
        }
    });

}

function LimpiarFormularioTipoMovimiento() {
    $("#lblIdTipoMov").text("");
    $("#txtNombreTipo").val("");
    $("#sltIngEgr").val("").trigger('change.select2');
    $("#sltEstadoTipo").val("").trigger('change.select2');
}

function GrabarTipo() {
    $('#frmTipoMov').parsley().validate();
    if ($('#frmTipoMov').parsley().isValid()) {
        tipomov.NOMBRE = $("#txtNombreTipo").val();
        tipomov.INGRESO_EGRESO = $("#sltIngEgr").val();
        tipomov.ESTADO = $("#sltEstadoTipo").val();
        var parametros = JSON.stringify(
            {
                precord: tipomov
            });
        $.ajax({
            type: "POST",
            traditional: true,
            datatype: "json",
            data: parametros,
            contentType: 'application/json; charset=utf-8',
            url: "/Configuracion/GrabarTipo",
            beforeSend: function () {
                run_waitMe($("#dlgTipoMov"), 'Grabando...');
            },
            success: function (data) {
                if (data.resultado == "error") {
                    swal({
                        type: 'error',
                        text: 'Error al grabar los datos. Aplicacion Msg : ' + data.mensaje,
                        confirmButtonColor: '#00BCD4'
                    });
                }
                else if (data.resultado == "success") {
                    $('#dlgTipoMov').modal('toggle');
                    swal({
                        type: 'success',
                        text: 'Datos grabados',
                        confirmButtonColor: '#00BCD4'
                    });
                    
                    CargaDatosTipo();
                }
                $("#dlgTipoMov").waitMe('hide');
                //$this.button('reset');
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $("#dlgTipoMov").waitMe('hide');
                //$this.button('reset');
                swal({
                    type: 'error',
                    text: 'Error en la llamada al servidor',
                    confirmButtonColor: '#00BCD4'
                });
            },
            complete: function () {

            }
        });
    }

}

var TipoMovimiento = function () {
    return {
        init: function () {

            tabTipoMov = $('#tabTipoMov').DataTable({
                "paging": false,
                "dom": 't',
                "autoWidth": false,
                "columnDefs":
                    [
                        { "targets": [0], "visible": false, "orderable": false },
                        {
                            "targets": [3],
                            render: function (data, type, row) {
                                var color = 'black';
                                if (data == 'Inactivo') {
                                    color = 'red';
                                }
                                return '<span style="color:' + color + '">' + data + '</span>'
                            }
                        },
                        { "targets": [4],/* "width": "10%",*/ "defaultContent": '<a href=#  class="clsedit"><i class="fa fa-pencil"></i></a>', "className" : "text-center" }
                    ],
                "data": ltipo,
                //"rowCallback": function (row, data, dataIndex) {
                //    if (data.ESTADO == "I") {
                //        $('td', row).addClass('registro_inactivo');
                //    }
                //},
                "columns": [
                    { "data": 'ID_TIPO_MOVIMIENTO' },
                    { "data": 'NOMBRE' },
                    { "data": 'TIPO_INGEGR' },
                    { "data": 'ESTADO_REGISTRO' }
                ]
            });

            CargaDatosTipo();

            tabTipoMov.on('click', 'a.clsedit', function (e) {
                var row = $(this).closest('tr');
                var data = tabTipoMov.row($(this).parents('tr')).data();
                var parametros = JSON.stringify({
                        pid: data.ID_TIPO_MOVIMIENTO
                });

                $.ajax({
                    dataType: 'JSON',
                    url: '/Configuracion/GetTipoMovimiento',
                    type: "POST",
                    contentType: 'application/json; charset=utf-8',
                    data: parametros,
                    success: function (result) {
                        if (result.resultado == "success") {
                            tipomov = result.data;
                            $("#dlgTipoMov").modal('toggle');
                            $('#frmTipoMov').parsley().reset();
                            //$("#txtIdTipoMov").val(tipomov.ID_TIPO_MOVIMIENTO);
                            $("#lblIdTipoMov").text(tipomov.ID_TIPO_MOVIMIENTO);
                            //$("#sltIngEgr").val(tipomov.INGRESO_EGRESO);
                            $("#sltIngEgr").val(tipomov.INGRESO_EGRESO).trigger('change.select2');
                            $("#txtNombreTipo").val(tipomov.NOMBRE);
                            $("#sltEstadoTipo").val(tipomov.ESTADO).trigger('change.select2');
                        } else if (result.resultado == "error") {
                            swal({
                                type: 'error',
                                text: 'Error al cargar los datos. Aplicacion Msg:' + data.msg,
                                confirmButtonColor: '#00BCD4'
                            });
                        }
                    },
                    error: function (result) {
                        $("#msgMain").html(result.responseText);
                        $("#msgMain").addClass("alert alert-danger");
                    }
                });
            });

            $("#butAceptaTipo").on("click", function () {
                GrabarTipo();
            });

            $("#butNuevoTipoMov").on("click", function () {
                tipomov = { ID_TIPO_MOVIMIENTO: 0, NOMBRE: "", ESTADO: "A" };
                LimpiarFormularioTipoMovimiento();
                $("#dlgTipoMov").modal('toggle');
            });
         
        }
    };
}();