var tablaPuntoReOrden;
var fechaInicio;
var fechaFin;


var Kardex = function () {
    return {
        init: function () {

            $("#FechaInicio").datepicker({
                format: "dd/mm/yyyy",
                todayHighlight: true,
                autoclose: true
            });

            $("#FechaFin").datepicker({
                format: "dd/mm/yyyy",
                todayHighlight: true,
                autoclose: true
            });

            fechaInicio = $("#FechaInicio").val();
            fechaFin = $("#FechaFin").val();
            if (fechaInicio == "" || fechaInicio == undefined) {
                fechaInicio = "vacio"
            }
            if (fechaFin == "" || fechaFin == undefined) {
                fechaFin = "vacio"
            }



            $("#btnConsult").click(function (event) {
                event.preventDefault();
                fechaInicio = $("#FechaInicio").val();
                fechaFin = $("#FechaFin").val();
                if (fechaInicio == "" || fechaInicio == undefined) {
                    fechaInicio = "vacio"
                }
                if (fechaFin == "" || fechaFin == undefined) {
                    fechaFin = "vacio"
                }
                tablaPuntoReOrden.ajax.reload();
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
                    return "(" + repo.CODIGO + ") " + repo.DESCRIPCION + " [" + repo.STOCK_ACTUAL + "]";
                },
                templateSelection: function (container) {
                    $(container.element).attr("data-codigo", container.CODIGO);
                    $(container.element).attr("data-item", container.DESCRIPCION);
                    $(container.element).attr("data-unidad", container.MEDIDA);
                    $(container.element).attr("data-idItem", container.id);
                    $(container.element).attr("data-stockactual", container.STOCK_ACTUAL);
                    return "(" + container.CODIGO + ") " + container.DESCRIPCION + " [" + container.STOCK_ACTUAL + "]";
                }
            });



            $("#btnExcel").click(function (event) {
                event.preventDefault();
                var fechaInicio = $("#FechaInicio").val();
                var fechaFin = $("#FechaFin").val();
                if (fechaInicio == "" || fechaInicio == undefined) {
                    fechaInicio = "vacio"
                }
                if (fechaFin == "" || fechaFin == undefined) {
                    fechaFin = "vacio"
                }
                window.location.href = '/Consulta/ExportToExcelPuntosReOrden?fechaInicio=' + fechaInicio + '&fechaFin=' + fechaFin;
            });

        }
    };
}();

