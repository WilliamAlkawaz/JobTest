import React from 'react'
import Category from './Category'

const Categories = ({ categories, onCatEdit, onSaveChnages, onCatDelete}) => {
    return (
        <>
            {categories.map(
                category=>{
                    return (<Category key={category.category_ID} categories={categories}
                        category={category} onCatEdit={onCatEdit} onSaveChnages={onSaveChnages}
                        onCatDelete={onCatDelete} />)
                }
            )}
            
        </>
    )
}

export default Categories