﻿var ldata = [];
var grupo;

function CargaDatos() {
    var $this = $(this);

    $.ajax({
        type: "POST",
        traditional: true,
        datatype: "json",
        data: null,
        contentType: 'application/json; charset=utf-8',
        url: "/Grupo/GetListaGrupo",
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
                var data = (data.data);
                tabGrupo.clear();
                tabGrupo.rows.add(data);
                tabGrupo.draw();
            }
            $(".box-body").waitMe('hide');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $this.button('reset');
            swal({
                type: 'error',
                text: 'Error al cargar los datos.',
                confirmButtonColor: '#00BCD4'
            });
        }
    });

}

function LimpiarFormularioGrupo() {
    $("#lblIdGrupo").text("");
    $("#txtNombre").val("");
    $("#txtCodigo").val("");
    $("#txtCuentaContable").val("");
    $("#sltEstado").val("");
}

function Grabar() {
    $('#frmGrupo').parsley().validate();
    if ($('#frmGrupo').parsley().isValid()) {
        grupo.NOMBRE = $("#txtNombre").val();
        grupo.CODIGO = $("#txtCodigo").val();
        grupo.CUENTA_CONTABLE = $("#txtCuentaContable").val();
        grupo.ESTADO = $("#sltEstado").val();
        var parametros = JSON.stringify(
            {
                precord: grupo
            });
        $.ajax({
            type: "POST",
            traditional: true,
            datatype: "json",
            data: parametros,
            contentType: 'application/json; charset=utf-8',
            url: "/Grupo/Grabar",
            beforeSend: function () {
                run_waitMe($("#dlgGrupo"), 'Grabando...');
            },
            success: function (data) {
                if (data.resultado == "error") {
                    swal({
                        type: 'error',
                        text: 'Error al grabar los datos. ' + data.mensaje,
                        confirmButtonColor: '#00BCD4'
                    });
                }
                else {
                    $('#dlgGrupo').modal('toggle');
                    swal({
                        type: 'success',
                        text: 'Datos grabados',
                        confirmButtonColor: '#00BCD4'
                    });

                    CargaDatos();
                }
                $("#dlgGrupo").waitMe('hide');
                //$this.button('reset');
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $("#dlgGrupo").waitMe('hide');
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

var Grupo = function () {
    return {
        init: function () {

            tabGrupo = $('#tabGrupo').DataTable({
                "paging": false,
                //"searching": false,
                //"filter": false,
                "dom": 't',
                "autoWidth": false,
                "columnDefs":
                    [
                        { "targets": [0],/* "width": "10%",*/ "visible": false, "orderable": false },
                        { "targets": [4],/* "width": "10%",*/ "defaultContent": '<button id="butEditar" type="button" class="btn btn-default btn-xs clsedit"><i class="fa fa-pencil"></i></button>' }
                    ],
                "data": ldata,
                //"rowCallback": function (row, data, dataIndex) {
                //    if (data.ESTADO == "I") {
                //        $('td', row).addClass('registro_inactivo');
                //    }
                //},
                "columns": [
                    { "data": 'ID_GRUPO' },
                    { "data": 'CODIGO' },
                    { "data": 'NOMBRE' },
                    { "data": 'CUENTA_CONTABLE' }
                ]
            });

            CargaDatos();

            tabGrupo.on('click', 'button.clsedit', function (e) {
                var row = $(this).closest('tr');
                var data = tabGrupo.row($(this).parents('tr')).data();
                var parametros = JSON.stringify({
                    pid: data.ID_GRUPO
                });

                $.ajax({
                    dataType: 'JSON',
                    url: '/Grupo/GetGrupo',
                    type: "POST",
                    contentType: 'application/json; charset=utf-8',
                    data: parametros,
                    success: function (result) {
                        grupo = result.data;
                        $("#dlgGrupo").modal('toggle');
                        $('#frmGrupo').parsley().reset();
                        $("#lblIdGrupo").text(grupo.ID_GRUPO);
                        $("#txtCodigo").val(grupo.CODIGO);
                        $("#txtNombre").val(grupo.NOMBRE);
                        $("#txtCuentaContable").val(grupo.CUENTA_CONTABLE);
                        $("#sltEstado").val(grupo.ESTADO);
                        $("#txtCodigo").prop("disabled", true);
                    },
                    error: function (result) {
                        $("#msgMain").html(result.responseText);
                        $("#msgMain").addClass("alert alert-danger");
                    }
                });
            });

            $("#butAcepta").on("click", function () {
                Grabar();
            });

            $("#butNuevoGrupo").on("click", function () {
                grupo = { ID_GRUPO: 0, CODIGO: "", NOMBRE: "", CUENTA_CONTABLE: "", ESTADO: "A" };
                $("#txtCodigo").prop("disabled", false);
                LimpiarFormularioGrupo();
                $("#dlgGrupo").modal('toggle');
            });

        }
    };
}();