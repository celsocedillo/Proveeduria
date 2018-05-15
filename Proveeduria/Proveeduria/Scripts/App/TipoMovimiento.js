var tabtipomov;
var ldata = [];
var tipomov;


function CargaDatos() {
    var $this = $(this);

    $.ajax({
        type: "POST",
        traditional: true,
        datatype: "json",
        data: null,
        contentType: 'application/json; charset=utf-8',
        url: "/TipoMovimiento/GetListaTipoMovimiento",
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
                //var items = data.items;
                var data = $.parseJSON(data.data);
                //tipomov = data[0]; //Esto es para guardar la estructura del objeto y poder usarlo en el momento de crear uno nuevo
                tabtipomov.clear();
                tabtipomov.rows.add(data);
                tabtipomov.draw();
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

function LimpiarFormularioTipoMovimiento() {
    $("#lblIdTipoMov").text("");
    $("#txtNombre").val("");
    $("#sltIngEgr").val("");
    $("#sltEstado").val("");
}

function Grabar() {
    $('#frmTipoMov').parsley().validate();
    if ($('#frmTipoMov').parsley().isValid()) {
        tipomov.NOMBRE = $("#txtNombre").val();
        tipomov.INGRESO_EGRESO = $("#sltIngEgr").val();
        tipomov.ESTADO = $("#sltEstado").val();
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
            url: "/TipoMovimiento/Grabar",
            beforeSend: function () {
                run_waitMe($("#dlgTipoMov"), 'Grabando...');
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
                    $('#dlgTipoMov').modal('toggle');
                    swal({
                        type: 'success',
                        text: 'Datos grabados',
                        confirmButtonColor: '#00BCD4'
                    });
                    
                    CargaDatos();
                }
                $("#dlgTipoMov").waitMe('hide');
                //$this.button('reset');
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $("#dlgTipoMov").waitMe('hide');
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



var TipoMovimiento = function () {
    return {
        init: function () {

            tabtipomov = $('#tabTipoMov').DataTable({
                //"paging": false,
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
                "rowCallback": function (row, data, dataIndex) {
                    if (data.ESTADO == "I") {
                        $('td', row).addClass('registro_inactivo');
                    }
                },
                "columns": [
                    { "data": 'ID_TIPO_MOVIMIENTO' },
                    { "data": 'NOMBRE' },
                    { "data": 'TIPO_INGEGR' },
                    { "data": 'ESTADO' }
                ]
            });

            CargaDatos();

            tabtipomov.on('click', 'button.clsedit', function (e) {
                var row = $(this).closest('tr');
                var data = tabtipomov.row($(this).parents('tr')).data();
                var parametros = JSON.stringify({
                        pid: data.ID_TIPO_MOVIMIENTO
                });

                $.ajax({
                    dataType: 'JSON',
                    url: '/TipoMovimiento/GetTipoMovimiento',
                    type: "POST",
                    contentType: 'application/json; charset=utf-8',
                    data: parametros,
                    success: function (result) {
                        tipomov = result.data;
                        $("#dlgTipoMov").modal('toggle');
                        $('#frmTipoMov').parsley().reset();
                        //$("#txtIdTipoMov").val(tipomov.ID_TIPO_MOVIMIENTO);
                        $("#lblIdTipoMov").text(tipomov.ID_TIPO_MOVIMIENTO);
                        $("#sltIngEgr").val(tipomov.INGRESO_EGRESO);
                        $("#txtNombre").val(tipomov.NOMBRE);
                        $("#sltEstado").val(tipomov.ESTADO);
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

            $("#butNuevoTipoMov").on("click", function () {
                tipomov = { ID_TIPO_MOVIMIENTO: 0, NOMBRE: "", ESTADO: "A" };
                LimpiarFormularioTipoMovimiento();
                $("#dlgTipoMov").modal('toggle');
            });
         
        }
    };
}();