"use client";

import React, { Suspense, useState, useEffect } from "react";
import qs from "query-string";
import { Search } from "lucide-react";
import { Input } from "./ui/input";
import { useRouter, usePathname, useSearchParams } from "next/navigation";
import { useDebounce } from "@/hooks/use-debounce";
import api from "@/lib/axios";
import { searchesForVocabularyAPI } from "@/actions/vocabulary-management";

// interface SearchInputProps {
// 	onSearch: (data: any) => void;
// }

const SearchInput = ({ onSearch }: { onSearch: (data: any) => void }) => {
	const [value, setValue] = useState("");
	const debouncedValue = useDebounce(value);
	const router = useRouter();
	const pathname = usePathname();

	useEffect(() => {
		if (debouncedValue) {
			handleWordToSearch();
		}
	}, [debouncedValue]);

	const handleWordToSearch = async () => {
		try {
			const { data } = await searchesForVocabularyAPI(value);
			console.log('Translation data:', data);
			if (data && typeof onSearch === 'function') {
				onSearch(data); // Invoke onSearch with the fetched data
			} else {
				console.error('No data returned or onSearch is not a function');
			}
		} catch (error) {
			console.error('Error fetching translation:', error);
		}
	};

	const onkeydown = (e: React.KeyboardEvent<HTMLInputElement>) => {
		if (e.key === "Enter") {
			handleWordToSearch();
		}
	};

	return (
		<div className="relative">
			<Search className="h-4 w-4 absolute top-3 left-3 text-slate-600" />
			<Input
				onChange={(e) => setValue(e.target.value)}
				value={value}
				className="w-full md:w-[300px] pl-9 rounded-full bg-slate-100 focus-visible:ring-slate-200"
				placeholder="Search for vocabulary"
				onKeyDown={onkeydown}
			/>

			<Suspense fallback={<div>Loading...</div>}>
				<SearchHandler debouncedValue={debouncedValue} pathname={pathname} router={router} />
			</Suspense>
		</div>
	);
};

const SearchHandler = ({ debouncedValue, pathname, router }: { debouncedValue: string, pathname: string, router: any }) => {
	const searchParams = useSearchParams();
	const currentCategoryId = searchParams.get("categoryId");

	useEffect(() => {
		const url = qs.stringifyUrl(
			{
				url: pathname,
				query: {
					categoryId: currentCategoryId,
					title: debouncedValue,
				},
			},
			{ skipEmptyString: true, skipNull: true }
		);

		if (router.asPath !== url) { // Prevent unnecessary pushes
			router.push(url);
		}
	}, [debouncedValue, currentCategoryId, router, pathname]);

	return null; // This component doesn't render anything itself
};

export default SearchInput;
