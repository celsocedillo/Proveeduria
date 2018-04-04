
function serializarForm() {
    var jsonObject = {
        invMedida: {
            UNI_CODIGO: $("#UNI_CODIGO").val(),
            UNI_DESCRIPCION: $("#UNI_DESCRIPCION").val()
        }
    };
    return jsonObject;
}

function limpiarForm() {
    $("#UNI_CODIGO").val("");
    $("#UNI_DESCRIPCION").val("");
}


function eliminar(id) {
    swal({
        title: 'Inventario Medida',
        text: '¿Seguro desea elimiar la medida?',
        type: 'question',
        showCancelButton: true,
        confirmButtonColor: '#00BCD4',
        cancelButtonColor: '#EF5350',
        confirmButtonText: 'Confirmar',
        cancelButtonText: 'Cancelar'
    }).then(function () {
            $.ajax({
                type: "POST",
                traditional: true,
                datatype: "json",
                data: { "id": id },
                url: '/InvMedida/Delete',
                success: function (data) {
                    if (!data.error) {
                        swal({
                            type: 'success',
                            title: 'Inventario Medida',
                            text: 'Eliminada con éxito',
                            confirmButtonColor: '#00BCD4'
                        });
                        window.location.href = "/InvMedida/Index";
                    }
                    else {
                        swal({
                            type: 'error',
                            title: 'Inventario Medida',
                            text: 'Existe un error al moento de crear la medida',
                            confirmButtonColor: '#00BCD4'
                        });
                    }
                },
                complete: function () {
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    swal({
                        type: 'error',
                        title: 'Inventario Medida',
                        text: 'Existe un error al moento de crear la medida',
                        confirmButtonColor: '#00BCD4'
                    });
                }
            });
    });
}



var InvMedida = function () {
    return {
        init: function () {

            $("#btnCreate").click(function (event) {
                event.preventDefault();
                $("#btnCreate").button('loading');//Ponemos a cargar el boton
                var data = JSON.stringify(serializarForm());//Serializando los datos
                $.ajax({
                    type: "POST",
                    datatype: "json",
                    data: data,
                    contentType: 'application/json; charset=utf-8',
                    url: $('#formCreate').attr("action"),
                    beforeSend: function () {
                        //$("#btnCreate").prop("disabled", true);
                    },
                    success: function (data) {
                        if (!data.error) {
                            limpiarForm();
                            swal({
                                type: 'success',
                                title: 'Inventario Medida',
                                text: 'Creada con éxito',
                                confirmButtonColor: '#00BCD4'
                            });
                        }
                        else {
                            swal({
                                type: 'error',
                                title: 'Inventario Medida',
                                text: 'Existe un error al moento de crear la medida',
                                confirmButtonColor: '#00BCD4'
                            });
                        }
                        //$("#btnCreate").("disabled", false);
                        $("#btnCreate").button('reset');
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        //$("#btnCreate").prop("disabled", false);
                        $("#btnCreate").button('reset');
                        swal({
                            type: 'error',
                            title: 'Inventario Medida',
                            text: 'Existe un error al moento de crear la medida',
                            confirmButtonColor: '#00BCD4'
                        });
                    }
                });
            });


            $("#btnEdit").click(function (event) {
                event.preventDefault();
                $("#btnEdit").button('loading');//Ponemos a cargar el boton
                var data = JSON.stringify(serializarForm());//Serializando los datos
                $.ajax({
                    type: "POST",
                    datatype: "json",
                    data: data,
                    contentType: 'application/json; charset=utf-8',
                    url: $('#formEdit').attr("action"),
                    beforeSend: function () {
                    },
                    success: function (data) {
                        if (!data.error) {
                            limpiarForm();
                            swal({
                                type: 'success',
                                title: 'Inventario Medida',
                                text: 'Actualizado con éxito',
                                confirmButtonColor: '#00BCD4'
                            });
                        }
                        else {
                            swal({
                                type: 'error',
                                title: 'Inventario Medida',
                                text: 'Existe un error al moento de actualiza la medida',
                                confirmButtonColor: '#00BCD4'
                            });
                        }
                        $("#btnEdit").button('reset');
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        $("#btnEdit").button('reset');
                        swal({
                            type: 'error',
                            title: 'Inventario Medida',
                            text: 'Existe un error al moento de actualizar la medida',
                            confirmButtonColor: '#00BCD4'
                        });
                    }
                });
            });

        }
    };
}();