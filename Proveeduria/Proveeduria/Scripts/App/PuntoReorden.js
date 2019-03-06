var tablaPuntoReOrden;
var fechaInicio;
var fechaFin;

function filtroNuevo() {
    localStorage.removeItem('Filtro');
    nombre = $("#nombre").val();
    activo = $("#EstadoID").val();
    destacado = $("#DestacadoID").val();
    categoria = parseInt($("#CategoriaID").val());
    inicio = $("#inicio").val();
    fin = $("#fin").val();
    if (nombre == "") { nombre = "vacio" }
    if (inicio == "") { inicio = "vacio" }
    if (fin == "") { fin = "vacio" }
    localStorage['Filtro'] = '{ "nombre": "' + nombre + '","activo": "' + activo + '","destacado": "' + destacado + '","categoria": "' + categoria + '","inicio": "' + inicio + '","fin": "' + fin + '" }';
    tablaLocales.ajax.reload();
}

function CargaDatos() {

    $.ajax({
        type: "POST",
        traditional: true,
        datatype: "json",
        data: parametros,
        contentType: 'application/json; charset=utf-8',
        url: "/Consulta/GetPuntoReOrden",
        beforeSend: function () {
            run_waitMe($(".content"), 'Cargando...');
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
                tablaPuntoReOrden.clear();
                tablaPuntoReOrden.rows.add(data.data);
                tablaPuntoReOrden.draw();
            }
            $(".content").waitMe('hide');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $(".content").waitMe('hide');
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


var PuntoReorden = function () {
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

            fecha1 = new Date(new Date().getFullYear(), 0, 1); // Con esto obtengo el primer dia del año
            fecha2 = new Date(); //fecha actual
            $("#txtFechaInicio").val(fecha1.toJSON().slice(0, 10).split("-").reverse().join("/"));
            $("#txtFechaFin").val(fecha2.toJSON().slice(0, 10).split("-").reverse().join("/"));

            fechaInicio = $("#FechaInicio").val();
            fechaFin = $("#FechaFin").val();
            if (fechaInicio == "" || fechaInicio == undefined) {
                fechaInicio = "vacio"
            }
            if (fechaFin == "" || fechaFin == undefined) {
                fechaFin = "vacio"
            }

            //tablaPuntoReOrden = $("#tablaPuntoReOrden").DataTable({
            //    autoWidth: false,
            //    processing: true,
            //    scrollX: true,
            //    serverSide: true,
            //    info: true,
            //    stateSave: true,
            //    ajax: {
            //        url: "/Consulta/GetPuntosReOrden",
            //        type: "POST",
            //        data: function (d) {
            //            d.inicio = fechaInicio,
            //            d.fin = fechaFin
            //        },
            //        error: function () {
            //        }
            //    },
            //    columns: [
            //        { data: "item", orderable: false },
            //        { data: "maximo", orderable: true },
            //        { data: "minimo", orderable: false },
            //        { data: "critica", orderable: true },
            //        { data: "inicio", orderable: true },
            //        { data: "actual", orderable: false },
            //        { data: "usado", orderable: false },
            //        { data: "messiete", orderable: false }
            //    ],
            //    "order": [[0, 'desc']]
            //});

            tablaPuntoReOrden = $("#tablaPuntoReOrden").DataTable({
                "autoWidth": false,
                "pageLength": 25,
                "rowCallback": function (nRow, aData, iDisplayIndex) {
                    if (aData["CANTIDAD_ACTUAL"] <= aData["CANTIDAD_CRITICA"]) {
                        //$('td', nRow).eq(2).css({ color: "#34485e" });
                        $('td', nRow).css("color" , "red");
                        //$(nRow).addClass('selected');
                    }
                    else if ((aData["CANTIDAD_ACTUAL"] <= aData["CANTIDAD_MINIMA "]) && (aData["CANTIDAD_ACTUAL"] > aData["CANTIDAD_CRITICA "])) {
                        $('td', nRow).css("color", "yellow");
                    }
                },
                "columnDefs":
                    [
                        { "targets": [2, 3, 4, 5, 6, 7, 8], "className": "text-right" }
                    ],
                "columns": [
                    { "data": "CODIGO", orderable: true },
                    { "data": "ITEM", orderable: true },
                    { "data": "CANTIDAD_MAXIMA", orderable: true },
                    { "data": "CANTIDAD_MINIMA", orderable: false },
                    { "data": "CANTIDAD_CRITICA", orderable: false },
                    { "data": "CANTIDAD_INICIO", orderable: false },
                    { "data": "CANTIDAD_ACTUAL", orderable: false },
                    { "data": "USADO", orderable: false },
                    { "data": "MES_ANTERIOR", orderable: false }
                ],
                "order": [[0, 'desc']]
            });


            $("#btnConsult").click(function (event) {
                //event.preventDefault();
                //fechaInicio = $("#FechaInicio").val();
                //fechaFin = $("#FechaFin").val();
                //if (fechaInicio == "" || fechaInicio == undefined) {
                //    fechaInicio = "vacio"
                //}
                //if (fechaFin == "" || fechaFin == undefined) {
                //    fechaFin = "vacio"
                //}
                //tablaPuntoReOrden.ajax.reload();

                if (ValidarFecha($("#txtFechaInicio").val()) && ValidarFecha($("#txtFechaFin").val())) {
                    parametros = JSON.stringify(
                        {
                            pFechaInicio: $("#txtFechaInicio").val(),
                            pFechaFin: $("#txtFechaFin").val(),
                            pTodos: $('input[name=oprTodos]:checked').val()
                        });
                    CargaDatos();
                } else {
                    swal({
                        type: 'error',
                        text: 'Fechas invalidas, revise el formato de las fechas',
                        confirmButtonColor: '#00BCD4'
                    });

                }


            });

            $("#btnExcel").click(function (event) {
                //var selected = $("select[name='sltItem']").find('option:selected');

                //xidItem = selected.data("iditem");
                //if (xidItem === undefined) {
                //    xidItem = "null";
                //}


                parametros = JSON.stringify(
                    {
                        pFechaInicio: $("#txtFechaInicio").val(),
                        pFechaFin: $("#txtFechaFin").val(),
                        pTodos: $('input[name=oprTodos]:checked').val()
                    });
                window.location.href = '/Consulta/GetPuntoReOrdenExcel?pFechaInicio=' + $("#txtFechaInicio").val() + '&pFechaFin=' + $("#txtFechaFin").val() + '&pTodos=' + $('input[name=oprTodos]:checked').val();
            });
        }
    };
}();
