

function Grabar() {
    $('#frmItem').parsley().validate();
    if ($('#frmItem').parsley().isValid()) {
        //var parametros = JSON.stringify(
        //    {
        //        precord: $("frmItem")
        //    });
        //$("#CODIGO").prop('disabled', false);
        var forma = serializaForma($("#frmItem"));
        $.ajax({
            type: "POST",
            traditional: true,
            datatype: "json",
            data: JSON.stringify(forma),
            contentType: 'application/json; charset=utf-8',
            url: "/Item/Grabar",
            beforeSend: function () {
                run_waitMe($(".box-primary"), 'Grabando...');
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
                    swal({
                        type: 'success',
                        text: 'Datos grabados',
                        timer: 20000,
                        confirmButtonColor: '#00BCD4'
                    });
                    window.location.href = '/Item/ListaItem';
                }
                $(".box-primary").waitMe('hide');
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $(".box-primary").waitMe('hide');
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

var Item = function () {
return {
    init: function () {

        $("#btnCancelar").on('click', function () {
            window.location.href = '/Item/ListaItem' ;
        });

        $("#btnGrabar").on('click', function () {
            Grabar();
        });

        if ($("#ESTADO").val() == "N") {

        } else {
            $("#CODIGO").prop('readonly', true);
        }

        
    }
};
}();