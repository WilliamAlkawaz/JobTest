import {useState, useEffect} from 'react'
import Header from './components/Header'
import Categories from './components/Categories'
import AddCategory from './components/AddCategory'
import EditRanges from './components/EditRanges'

const App = () => {
    const [showAddCat, setShowAddCat] = useState(false);
    const [showEditCat, setShowEditCat] = useState(false);
    const [catDeleted, setCatDeleted] = useState(false);
    const [categories, setCategories] = useState([]);

    useEffect(() => {
        fetchCategories();
    }, []);

    const fetchCategories = () => {
        fetch('https://localhost:44364/Api/CategoriesApi').then(res => res.json())
            .then(
                (result) => {
                    result.map(category => {
                        category.imSource = 'data:' + category.imageMimeType + ';base64,' + category.photoFile;
                    })
                    setCategories(result);
                    return result;
                }
            );
    }

    const determineRanges = (cats) => {
        var r = [];
        for (var i = 0; i <= cats.length; i++) {
            if (i === cats.length)
                r.push(cats[i - 1].max);
            else
                r.push(cats[i].min);
        }
        return r;
    }

    const addClicked = () => {
        setShowAddCat(!showAddCat);
    }

    const editClicked = () => {
        setShowEditCat(!showEditCat);
    }

    // Only name and image can be updated.
    const editCategory = (newCat, im) => {
        let formData = new FormData();
        let file = im.files[0];
        formData.append('MyImage', file);
        formData.append('Category_ID', newCat.category_ID);
        formData.append('Name', newCat.name);
        formData.append('Min', newCat.min);
        formData.append('Max', newCat.max);

        const requestOptions = {
            method: 'PUT',
            body: formData
        };
        fetch('https://localhost:44364/api/CategoriesApi/' + newCat.category_ID, requestOptions)
            .catch((error) => {
                console.log(error)
            })
        for (var i = 0; i < categories.length; i++) {
            if (newCat.category_ID === categories[i].category_ID) {
                categories[i].name = newCat.name;
                categories[i].imSource = newCat.imSource;
            }
        }
        setCategories(categories);
    }

    const deleteCat = (id) => {
        if (window.confirm('Are you sure you want to delete this?')) {
            const requestOptions = {
                method: 'DELETE',
            };
            fetch('https://localhost:44364/api/CategoriesApi/' + id, requestOptions)
                .catch((error) => {
                    console.log(error)
                })
            for (var i = 0; i < categories.length; i++) {
                if (categories[i].category_ID === id) {
                    if (i === 0) {
                        categories[i + 1].min = 0;
                    }
                    else if (i === (categories.length - 1)) {
                        categories[i - 1].max = categories[i].max;
                    }
                    else {
                        categories[i - 1].max = categories[i - 1].max + Math.round((categories[i].max - categories[i].min) / 2);
                        categories[i + 1].min = categories[i - 1].max;
                    }
                    break;
                }
            }            
            setCategories(categories.filter(category1 => category1.category_ID !== id));
        }
    }    

    const addCategory = (newCat, im) => {        
        let formData = new FormData();
        let file = im.files[0];
        formData.append('MyImage', file);
        formData.append('Name', newCat.name);
        formData.append('Min', newCat.min);
        formData.append('Max', newCat.max);

        const requestOptions = {
            method: 'POST',
            body: formData
        };
        fetch('https://localhost:44364/api/CategoriesApi/', requestOptions)
            .catch((error) => {
                console.log(error)
            })
        categories[categories.length - 1].max = newCat.min;
        let fff = [...categories, newCat];
        setCategories(fff);
        setShowAddCat(false);
    }

    // When editing ranges it has to be done for all the categories as follows: 
    // 1. Update all the ranges. 
    // 2. Update vehicles ranges. 
    const editRanges = (ranges) => {
        if ((Math.min(...ranges) > 0) || (Math.max(...ranges) < categories[categories.length - 1].max)) {
            alert('Minimum cannot not be bigger than zero and Maximum cannot be less than the maximum value');
            return;
        }
        let nc = [];
        for (var i = 0; i < categories.length; i++) {
            let cc = {
                id: categories[i].id,
                name: categories[i].name,
                min: ranges[i],
                max: ranges[i + 1],
                imSource: categories[i].imSource,
                photoFile: categories[i].photoFile
            }
            nc = [...nc, cc]
        }

        let formData = new FormData();
        
        formData.append('Ranges', ranges);

        const requestOptions = {
            method: 'POST',
            body: formData
        };
        fetch('https://localhost:44364/api/CategoriesApi/PostNewRanges/', requestOptions)
            .catch((error) => {
                console.log(error)
            })
        setCategories(nc);
        setShowEditCat(false);
    }

    return (
        <div className="container">
            <a href='https://localhost:44364/Categories'>Back to list</a>
            <Header title='Categories List' onAdd1={addClicked} onEdit={editClicked} showAdd={showAddCat} showEdit={showEditCat} />
            {showAddCat && <AddCategory addCategory={addCategory} ranges={determineRanges(categories)} setShowAddCat={setShowAddCat} />}
            {showEditCat && <EditRanges ranges={determineRanges(categories)} onEdit={editRanges} />}

            {
                categories.length > 0 ? <Categories categories={categories} onCatEdit={editCategory}
                    setCatDeleted={setCatDeleted} catDeleted={catDeleted} onCatDelete={deleteCat} /> : 'No categories to show'
            }

        </div>
    );
}

export default App;