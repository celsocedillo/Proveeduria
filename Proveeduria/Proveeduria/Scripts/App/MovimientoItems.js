var tablaMovimientos;
var inicio;
var fin;
var idItem;
var anioMovimiento;
var numeroMovimiento;
var tipoMovimiento;
var parametros;

function CargaDatos() {

    $.ajax({
        type: "POST",
        traditional: true,
        datatype: "json",
        data: parametros,
        contentType: 'application/json; charset=utf-8',
        url: "/Consulta/GetMovimientos",
        beforeSend: function () {
            run_waitMe($(".box"), 'Cargando...');
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
                tabMovimiento.clear();
                tabMovimiento.rows.add(data.data);
                tabMovimiento.draw();
            }
            $(".box").waitMe('hide');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $(".box").waitMe('hide');
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

function Excel() {

}

var MovimientoItems = function () {
    return {
        init: function () {

            $.fn.dataTable.moment('DD/MM/YYYY');

            $("#txtFechaInicio").datepicker({
                format: "dd/mm/yyyy",
                todayHighlight: true,
                autoclose: true
            });

            $("#txtFechaFin").datepicker({
                format: "dd/mm/yyyy",
                todayHighlight: true,
                autoclose: true
            });


            tabMovimiento = $("#tabMovimiento").DataTable({
                "autoWidth": false,
                "pageLength": 25,
                //ajax: {
                //    url: "/Consulta/ConsultaMovimiento",
                //    type: "POST",
                //    data: function (d) {
                //        d.inicio = inicio,
                //            d.fin = fin,
                //            d.anioMovimiento = anioMovimiento,
                //            d.numeroMovimiento = numeroMovimiento,
                //            d.idItem = idItem,
                //            d.tipoMovimiento = tipoMovimiento
                //    },
                //    error: function () {
                //    }
                //},
                "columnDefs": [{ "targets": [5, 6], "className": "text-right" }],
                "columns": [
                    { "data": "ANIO", orderable: true },
                    { "data": "NUMERO_MOVIMIENTO", orderable: true },
                    { "data": "FECHA_SOLICITUD", orderable: false },
                    { "data": "TIPO_MOVIMIENTO", orderable: true },
                    { "data": "ITEM", orderable: false },
                    { "data": "CANTIDAD_MOVIMIENTO", orderable: false },
                    { "data": "COSTO_MOVIMIENTO", orderable: false }
                    
                ],
                "order": [[0, 'desc']]
            });

            $('#sltItem').select2({
                language: 'es',
                reverse: false,
                ajax: {
                    url: '/Item/SearchItemsFiltro',
                    dataType: "json",
                    type: "POST",
                    contentType: 'application/json; charset=utf-8',
                    quietMillis: 100,
                    data: function (params) {
                        let parametros = JSON.stringify({ "filtro": params.term, "pagina": 10 });
                        return parametros;
                    },
                    processResults: function (data, params) {
                        var array = data.items;
                        return { results: array };
                    }
                },
                escapeMarkup: function (markup) { return markup; },
                //minimumInputLength: 1,
                templateResult: function (repo) {
                    return "(" + repo.CODIGO + ") " + repo.DESCRIPCION;
                },
                templateSelection: function (container) {
                    $(container.element).attr("data-codigo", container.CODIGO);
                    $(container.element).attr("data-descripcion", container.DESCRIPCION);
                    $(container.element).attr("data-unidad", container.MEDIDA);
                    $(container.element).attr("data-iditem", container.id);
                    return "(" + container.CODIGO + ") " + container.DESCRIPCION;
                }
            });


            $("#btnConsult").click(function (event) {
                var selected = $("select[name='sltItem']").find('option:selected');

                //event.preventDefault();
                //inicio = $("#FechaInicio").val();
                //fin = $("#FechaFin").val();
                //idItem = selected.data("iditem");
                //anioMovimiento = $("#anioMovimiento").val();
                //numeroMovimiento = $("#numeroMovimiento").val();
                //tipoMovimiento = $("#selIdTipoMovimiento").val();
                var xinicio = $("#txtFechaInicio").val();

                parametros = JSON.stringify(
                    {
                        inicio: $("#txtFechaInicio").val(),
                        fin : $("#txtFechaFin").val(),
                        idItem : selected.data("iditem"),
                        anioMovimiento : $("#txtAnioMovimiento").val(),
                        numeroMovimiento : $("#txtNumeroMovimiento").val(),
                        tipoMovimiento : $("#selIdTipoMovimiento").val()
                    });
                CargaDatos();
                //if (inicio == "" || inicio == undefined) {
                //    inicio = "vacio";
                //}
                //if (fin == "" || fin == undefined) {
                //    fin = "vacio";
                //}
                //if (idItem == "" || idItem == undefined) {
                //    idItem = "vacio";
                //}
                //if (codigoMovimiento == "" || codigoMovimiento == undefined) {
                //    codigoMovimiento = "vacio";
                //}                
                //if (tipoMovimieno == "" || tipoMovimieno == undefined) {
                //    tipoMovimieno = "vacio";
                //}
                //tablaMovimientos.ajax.reload();
            });

            $("#btnExcel").click(function (event) {
                var selected = $("select[name='sltItem']").find('option:selected');

                xidItem = selected.data("iditem");
                if (xidItem === undefined) {
                    xidItem = "null";
                }
                    

                parametros = JSON.stringify(
                    {
                        inicio: $("#txtFechaInicio").val(),
                        fin: $("#txtFechaFin").val(),
                        idItem: selected.data("iditem"),
                        anioMovimiento: $("#txtAnioMovimiento").val(),
                        numeroMovimiento: $("#txtNumeroMovimiento").val(),
                        tipoMovimiento: $("#selIdTipoMovimiento").val()
                    });
                window.location.href = '/Consulta/GetMovimientosExcel?inicio=' + $("#txtFechaInicio").val() + '&fin=' + $("#txtFechaFin").val() + '&idItem=' + xidItem + '&anioMovimiento=' + $("#txtAnioMovimiento").val() + '&numeroMovimiento=' + $("#txtNumeroMovimiento").val() + '&tipoMovimiento=' + $("#selIdTipoMovimiento").val();
            });

        }
    };
}();