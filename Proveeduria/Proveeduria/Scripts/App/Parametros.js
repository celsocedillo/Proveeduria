var tabtipomov;
var tabMedida;
var ldata = [];
var tipomov;

var lmedida = [];
var medida;




function CargaDatosTipoMovimiento() {
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
                //var data = $.parseJSON(data.data);
                var data = (data.data);
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

function CargaDatosMedidas() {
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
                var data = (data.data);
                //tipomov = data[0]; //Esto es para guardar la estructura del objeto y poder usarlo en el momento de crear uno nuevo
                tabMedida.clear();
                tabMedida.rows.add(data);
                tabMedida.draw();
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
        }
    });

}

function LimpiarFormularioTipoMovimiento() {
    $("#lblIdTipoMov").text("");
    $("#txtNombre").val("");
    $("#sltIngEgr").val("");
    $("#sltEstado").val("");
}

var Parametros = function () {
    return {
        init: function () {

            tabtipomov = $('#tabTipoMov').DataTable({
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

            CargaDatosTipoMovimiento();

            tabtipomov.on('click', 'button.clsedit', function (e) {
                var row = $(this).closest('tr');
                var data = tabTipoMov.row($(this).parents('tr')).data();
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


            tabMedida = $('#tabMedida').DataTable({
                //"filter": false,
                "dom": 't',
                "autoWidth": false,
                "columnDefs":
                    [
                        { "targets": [0],/* "width": "10%",*/ "visible": true, "orderable": false },
                        { "targets": [2],/* "width": "10%",*/ "defaultContent": '<a id="butEditar" class="clsedit"><i class="fa fa-pencil"></i></a>' }
                    ],
                "data": ldata,
                //"rowCallback": function (row, data, dataIndex) {
                //},
                "columns": [
                    { "data": 'ID_MEDIDA' },
                    { "data": 'NOMBRE' }
                ]
            });

            CargaDatosMedidas();

            $("#butAceptaMedida").on("click", function () {
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


        }
    };
}();