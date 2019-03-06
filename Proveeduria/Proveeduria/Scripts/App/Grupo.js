var lgrupo = [];
var grupo;

function CargaDatosGrupo() {
    var $this = $(this);

    $.ajax({
        type: "POST",
        traditional: true,
        datatype: "json",
        data: null,
        contentType: 'application/json; charset=utf-8',
        url: "/Configuracion/GetListaGrupo",
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
    $("#txtNombreGrupo").val("");
    $("#txtCodigo").val("");
    $("#txtCuentaContable").val("");
    $("#sltEstadoGrupo").val("").trigger('change.select2');
}

function GrabarGrupo() {
    $('#frmGrupo').parsley().validate();
    if ($('#frmGrupo').parsley().isValid()) {
        grupo.NOMBRE = $("#txtNombreGrupo").val();
        grupo.CODIGO = $("#txtCodigo").val();
        grupo.CUENTA_CONTABLE = $("#txtCuentaContable").val();
        grupo.ESTADO = $("#sltEstadoGrupo").val();
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
            url: "/Configuracion/GrabarGrupo",
            beforeSend: function () {
                run_waitMe($("#dlgGrupo"), 'Grabando...');
            },
            success: function (data) {
                if (data.resultado == "error") {
                    swal({
                        type: 'error',
                        text: 'Error al grabar los datos. ' + data.msg,
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

                    CargaDatosGrupo();
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
                "dom": 't',
                "autoWidth": false,
                "columnDefs":
                    [
                        { "targets": [0],/* "width": "10%",*/ "visible": false, "orderable": false },
                        {
                            "targets": [4],
                            render: function (data, type, row) {
                                var color = 'black';
                                if (data == 'Inactivo') {
                                    color = 'red';
                                }
                                return '<span style="color:' + color + '">' + data + '</span>'
                            }
                        },
                        { "targets": [5],/* "width": "10%",*/ "defaultContent": '<a href=# class="clsedit"><i class="fa fa-pencil"></i></a>', "className" : "text-center" }
                    ],
                "data": lgrupo,
                "columns": [
                    { "data": 'ID_GRUPO' },
                    { "data": 'CODIGO' },
                    { "data": 'NOMBRE' },
                    { "data": 'CUENTA_CONTABLE' },
                    { "data": 'ESTADO_REGISTRO' }
                ]
            });

            CargaDatosGrupo();

            tabGrupo.on('click', 'a.clsedit', function (e) {
                var row = $(this).closest('tr');
                var data = tabGrupo.row($(this).parents('tr')).data();
                var parametros = JSON.stringify({
                    pid: data.ID_GRUPO
                });

                $.ajax({
                    dataType: 'JSON',
                    url: '/Configuracion/GetGrupo',
                    type: "POST",
                    contentType: 'application/json; charset=utf-8',
                    data: parametros,
                    success: function (result) {
                        grupo = result.data;
                        $("#dlgGrupo").modal('toggle');
                        $('#frmGrupo').parsley().reset();
                        $("#lblIdGrupo").text(grupo.ID_GRUPO);
                        $("#txtCodigo").val(grupo.CODIGO);
                        $("#txtNombreGrupo").val(grupo.NOMBRE);
                        $("#txtCuentaContable").val(grupo.CUENTA_CONTABLE);
                        $("#sltEstadoGrupo").val(grupo.ESTADO).trigger('change.select2');
                        $("#txtCodigo").prop("disabled", true);
                    },
                    error: function (result) {
                        $("#msgMain").html(result.responseText);
                        $("#msgMain").addClass("alert alert-danger");
                    }
                });
            });

            $("#butAceptaGrupo").on("click", function () {
                GrabarGrupo();
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