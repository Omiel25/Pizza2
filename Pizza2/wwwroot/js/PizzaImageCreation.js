//----------- BEGIN Shopcart_Script -----------
var cartContainer = document.getElementById("cart-container");
var dataContainer = document.getElementsByName("data-container");
var orderCount = document.getElementById("order-count");

var cartSizeCount = 0;

dataContainer.forEach((container, index) => {
    var btn = container.querySelector("#add-to-cart-btn");
    var data = container.querySelector("#data");
    var pizzaName = container.querySelector("#data-name");

    btn.addEventListener("click", (index) => {
        var id = data.value;
        var name = pizzaName.value;

        var newInput = "";

        newInput = `<input type="number" name="pizza-${cartSizeCount}" value="${id}" id="data" hidden>`;

        cartContainer.innerHTML += newInput;

        cartSizeCount++;

        //Show alert with success message
        swal({
            icon: "success",
            title: "Success",
            text: `Succesfully added ${name} to the cart!`
        });

        //Refresh order count
        orderCount.innerHTML = `${cartSizeCount} - `;
    });
});
//----------- END Shopcart_Script -----------

//----------- BEGIN Pizza_Img_Creation -----------
let imageContainer = document.getElementsByName("pizza-image-container");
let customPizzaImagesContainer = document.querySelectorAll("#custom-pizza-images");

let leftPosition = 0;
let topPosition = 0;
let zPosition = 1;

if (document.readyState) {
    imageContainer.forEach(container => {
        let containerRect = container.getBoundingClientRect();
        let containerHeight = containerRect.bottom - containerRect.top;
        console.log("Container height: " + containerHeight);
        console.log("containerRect.bottom: " + containerRect.bottom);
        console.log("containerRect.top: " + containerRect.top);

        let images = container.querySelectorAll("img");

        images.forEach(image => {
            if (zPosition == 1) {

                leftPosition = containerRect.left;
                topPosition = containerRect.top;

                image.style = `position:absolute; left:${leftPosition}; top:${topPosition}; height:${containerHeight}px;`;
                image.style.zIndex = zPosition;
            } else {
                image.style = `position:absolute; left:${leftPosition}; top:${topPosition}; height:${containerHeight}px;`;
                image.style.zIndex = zPosition;
            }
            zPosition++;
        });
    });

    customPizzaImagesContainer.forEach(container => {
        let containerRect = container.parentElement.getBoundingClientRect();
        let containerHeight = containerRect.bottom - containerRect.top;

        let images = container.querySelectorAll("img");

        images.forEach(image => {
            if (zPosition == 1) {

                leftPosition = containerRect.left;
                topPosition = containerRect.top;

                image.style = `position:absolute; left:${leftPosition}; top:${topPosition}; height:${containerHeight}px;`;
                image.style.zIndex = zPosition;
            } else {
                image.style = `position:absolute; left:${leftPosition}; top:${topPosition}; height:${containerHeight}px;`;
                image.style.zIndex = zPosition;
            }
            zPosition++;
        });
    });
}
//----------- END Pizza_Creation -----------

//------------------------------------------
let customPizzaImages = document.querySelectorAll("#custom-pizza-images .pizza-ingridient-image");
let customPizzaInputs = document.querySelectorAll("#custom-pizza-ingridients input");
let customPizzaForm = document.querySelector("#custom-pizza-form-container #custom-pizza-form");

let customPizzaIngridients = [];

//Set all ingridients visibility to hidden
customPizzaImages.forEach(image => {
    image.style.visibility = "hidden";
});

//Change images visibilities on input change and update price
customPizzaInputs.forEach(input => {
    //Change images visibilities
    input.addEventListener("click", (index) => {
        let customPizzaInputID = input.id;
        let customPizzaInputChecked = input.checked;

        let skipLoop = false;
        customPizzaImages.forEach(img => {
            if (skipLoop == true) {
                return;
            }

            if (img.id == customPizzaInputID) {
                img.style.visibility = customPizzaInputChecked ? 'visible' : 'hidden';
                if (customPizzaInputID.includes("Sauce")) {
                    //disable other sauces
                    customPizzaImages.forEach(ingridientImage => {
                        if (ingridientImage.id.includes("Sauce") && ingridientImage.id != img.id) {
                            ingridientImage.style.visibility = "hidden";
                        }
                    });
                }

                skipLoop = true;
            }

        });

        //Update price and put ingridients + basket pizzas in input
        let customPizzaForm = document.querySelector("#custom-pizza-form-container #custom-pizza-form");
        let customPizzaFormIngridientsContainer = document.querySelector("#custom-pizza-form-container #custom-pizza-form-ingridients");
        customPizzaFormIngridientsContainer.innerHTML = "";

        var ingridients = JSON.parse('@Html.Raw(Json.Serialize(Model.Ingridients))');
        var pizzaPrice = JSON.parse('@Html.Raw( Json.Serialize( startingPrice ) )');

        //Reset custom pizza ingridients
        customPizzaIngridients = [];

        customPizzaInputs.forEach(box => {
            if (box.checked) {
                //Set price
                let skipLoop = false;
                let ingridientName = box.id;
                ingridients.forEach(ingridient => {
                    if (skipLoop) {
                        return;
                    }

                    if (ingridient.ingridientName != "Pizza Pie" && ingridient.ingridientName == ingridientName) {
                        let ingridientPrice = ingridient.ingridientPrice;
                        pizzaPrice += ingridientPrice;
                        skipLoop = true;

                        //Put ingridients inside form container
                        var newInput = "";

                        newInput = `<input type="number" name="${ingridient.ingridientName}" value="${ingridient.id}" hidden>`;

                        customPizzaFormIngridientsContainer.innerHTML += newInput;
                        customPizzaIngridients.push(ingridient.ingridientName);
                    }
                });
            }
        });

        let customPizzaPrice = document.querySelector("#custom-pizza-form-container #custom-pizza-price");
        let customPizzaPriceInput = document.querySelector("#custom-pizza-form-container #custom-pizza-price-input");

        pizzaPrice = (Math.round(pizzaPrice * 100) / 100).toFixed(2);

        customPizzaPrice.innerHTML = `<span class='py-2'> Price: ${pizzaPrice} PLN</span>`;

        customPizzaPriceInput.value = pizzaPrice;
    });
});

//Fire ajax request to server - AddCustomPizza - and add result to the basket
let customPizzaFormInputs = document.querySelectorAll("#custom-pizza-form-ingridients input")
let customPizzaFormButton = document.querySelector("#add-custom-pizza-btn");
let pizzaCart = document.querySelectorAll("#cart-container input");
let pizzaNameInput = document.querySelector("#pizza-name");

function AddCustomPizza(event) {
    let pizzaName = pizzaNameInput.value;

    $.ajax({
        url: '/Home/createCustomPizza',
        type: 'post',
        traditional: true,
        data: { ingridients: customPizzaIngridients, name: pizzaName },
        dataType: 'json',
        success: function (data) {
            var newInput = "";

            newInput = `<input type="number" name="pizza-${cartSizeCount}" value="${data.addedPizzaID}" id="data" hidden>`;

            cartContainer.innerHTML += newInput;

            cartSizeCount++;

            //Show alert with success message
            swal({
                icon: "success",
                title: "Success",
                text: `Succesfully added Your own pizza to the cart!`
            });

            //Refresh order count
            orderCount.innerHTML = `${cartSizeCount} - `;
        },
        error: function (data) {
            alert("Something went wrong...");
        }
    });
}

customPizzaFormButton.addEventListener("click", AddCustomPizza);
/*
1.Get ID of the input
2.Get img with the same id
3.Make img hidden/visible
*/
